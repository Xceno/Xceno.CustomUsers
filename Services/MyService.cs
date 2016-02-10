namespace Xceno.CustomUsers.Services
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using System.Xml.Linq;
	using Orchard;
	using Orchard.ContentManagement;
	using Orchard.Core.Common.Models;
	using Orchard.DisplayManagement;
	using Orchard.Localization;
	using Orchard.Logging;
	using Orchard.Messaging.Services;
	using Orchard.Security;
	using Orchard.Services;
	using Orchard.Users.Events;
	using Orchard.Users.Models;
	using Events;
	using Helpers;
	using Models;
	using ViewModels.Editor;

	public class MyService : IMyService
	{
		private const string PBKDF2 = "PBKDF2";
		private readonly IClock clock;
		private readonly IEncryptionService encryptionService;
		private readonly ICustomUserEventHandler customUserEventHandler;
		private readonly IMembershipService membershipService;
		private readonly IMessageService messageService;
		private readonly IOrchardServices orchardServices;
		private readonly IShapeDisplay shapeDisplay;
		private readonly IShapeFactory shapeFactory;
		private readonly IUserEventHandler userEventHandler;

		public MyService(
			IMembershipService membershipService,
			IUserEventHandler userEventHandler,
			IOrchardServices orchardServices,
			IShapeFactory shapeFactory,
			IShapeDisplay shapeDisplay,
			IMessageService messageService,
			ICustomUserEventHandler customUserEventHandler,
			IEncryptionService encryptionService,
			IClock clock)
		{
			this.orchardServices = orchardServices;
			this.membershipService = membershipService;
			this.userEventHandler = userEventHandler;
			this.shapeFactory = shapeFactory;
			this.shapeDisplay = shapeDisplay;
			this.messageService = messageService;
			this.customUserEventHandler = customUserEventHandler;
			this.encryptionService = encryptionService;
			this.clock = clock;
			this.Logger = NullLogger.Instance;
			this.T = NullLocalizer.Instance;
		}

		public ILogger Logger { get; set; }

		public Localizer T { get; set; }

		/// <summary>
		/// Quick access to the ContentManager. Just for convenience...
		/// </summary>
		public IContentManager ContentManager
		{
			get { return this.orchardServices.ContentManager; }
		}

		public CustomUserPart CreateCustomUser(CustomUserCreate customUserCreate)
		{
			this.Logger.Information("CreateCustomUser {0}", customUserCreate.UserName);

			var customUser = this.ContentManager.New("CustomUser");
			var customUserPart = customUser.As<CustomUserPart>();
			customUserPart.ActivationCode = customUserCreate.ActivationCode;
			customUserPart.FirstName = customUserCreate.FirstName.TrimSafe();
			customUserPart.LastName = customUserCreate.LastName.TrimSafe();
			customUserPart.WelcomeText = customUserCreate.WelcomeText;

			var user = this.ContentManager.New<UserPart>("User");
			user.UserName = customUserCreate.UserName.TrimSafe();
			user.NormalizedUserName = customUserCreate.UserName.TrimSafe().ToLowerInvariant();
			user.Email = customUserCreate.Email != null ? customUserCreate.Email.Trim().ToLowerInvariant() : null;
			user.HashAlgorithm = PBKDF2;
			user.RegistrationStatus = UserStatus.Pending;
			user.EmailStatus = UserStatus.Pending;
			this.ContentManager.Create(user);

			customUserPart.User = user;

			customUser.As<CommonPart>().Owner = user;

			var userContext = new UserContext
							  {
								  User = customUserPart.User,
								  Cancel = false,
								  UserParameters = new CreateUserParams(customUserCreate.UserName, null, null, null, null, false)
							  };

			this.userEventHandler.Creating(userContext);

			if ( userContext.Cancel )
			{
				return null;
			}

			this.ContentManager.Create(customUser);
			this.userEventHandler.Created(userContext);
			this.customUserEventHandler.CustomUserCreated(customUserPart);

			return customUserPart;
		}

		public CustomUserPart ActivateCustomUser(int userId, CreateUserParams createUserParams)
		{
			this.Logger.Information("ActivateCustomUser {0}", createUserParams.Email);

			if ( string.IsNullOrWhiteSpace(createUserParams.Email) )
			{
				throw new ArgumentException("No email adress provided!");
			}

			var customUserPart = this.GetCustomUser(userId);
			var normalizedEmailAddress = createUserParams.Email.Trim().ToLowerInvariant();

			// Prevent unnecessary override
			if ( customUserPart.User.Email != normalizedEmailAddress )
			{
				customUserPart.User.Email = normalizedEmailAddress;
			}

			this.membershipService.SetPassword(customUserPart.User, createUserParams.Password);

			var registrationSettings = this.orchardServices.WorkContext.CurrentSite.As<RegistrationSettingsPart>();
			if ( registrationSettings != null )
			{
				customUserPart.User.RegistrationStatus = registrationSettings.UsersAreModerated
					? UserStatus.Pending
					: UserStatus.Approved;

				customUserPart.User.EmailStatus = registrationSettings.UsersMustValidateEmail
					? UserStatus.Pending
					: UserStatus.Approved;
			}

			if ( createUserParams.IsApproved )
			{
				customUserPart.User.RegistrationStatus = UserStatus.Approved;
				customUserPart.User.EmailStatus = UserStatus.Approved;
			}

			if ( customUserPart.User.RegistrationStatus == UserStatus.Approved )
			{
				this.userEventHandler.Approved(customUserPart.User);
			}

			if ( registrationSettings != null && registrationSettings.UsersAreModerated && registrationSettings.NotifyModeration
				 && !createUserParams.IsApproved )
			{
				var usernames = string.IsNullOrWhiteSpace(registrationSettings.NotificationsRecipients)
					? new string[0]
					: registrationSettings.NotificationsRecipients.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

				foreach ( var userName in usernames )
				{
					if ( string.IsNullOrWhiteSpace(userName) )
					{
						continue;
					}

					var recipient = this.membershipService.GetUser(userName);
					if ( recipient != null )
					{
						var template = this.shapeFactory.Create("Template_User_Moderated", Arguments.From(createUserParams));
						template.Metadata.Wrappers.Add("Template_User_Wrapper");

						var parameters = new Dictionary<string, object>
										 {
											 { "Subject", this.T("New account").Text },
											 { "Body", this.shapeDisplay.Display(template) },
											 { "Recipients", new[] { recipient.Email } }
										 };

						this.messageService.Send("Email", parameters);
					}
				}
			}

			this.customUserEventHandler.CustomUserActivated(customUserPart);

			return customUserPart;
		}

		public void DeleteCustomUser(int customUserId)
		{
			var customUserPart = this.ContentManager.Get<CustomUserPart>(customUserId);
			if ( customUserPart == null )
			{
				throw new ArgumentException(this.T("The specified user doesn't exist.").Text, "customUserId");
			}

			if ( customUserPart.User != null )
			{
				if ( string.Equals(this.orchardServices.WorkContext.CurrentSite.SuperUser, customUserPart.User.UserName, StringComparison.Ordinal) )
				{
					throw new InvalidOperationException(this.T("The Super user can't be removed. Please disable this account or specify another Super user account.").Text, new ArgumentException(this.T("Invalid userId. You can't delete the super user!").Text, "customUserId"));
				}

				this.ContentManager.Remove(customUserPart.User.ContentItem);
			}

			this.ContentManager.Remove(customUserPart.ContentItem);
		}

		public IEnumerable<CustomUserPart> GetCustomUsers()
		{
			return this.ContentManager.Query<CustomUserPart, CustomUserRecord>().List();
		}

		public CustomUserPart GetCustomUser(int id)
		{
			var item = this.ContentManager.Get(id, VersionOptions.AllVersions);
			if ( item != null && item.TypeDefinition.Name == "CustomUser" )
			{
				return item.As<CustomUserPart>();
			}

			if ( item != null && item.TypeDefinition.Name == "User" )
			{
				var query = this.ContentManager.Query<CustomUserPart, CustomUserRecord>("CustomUser")
					.Where(x => x.UserPartRecord != null && x.UserPartRecord.Id == id)
					.List();

				return query.FirstOrDefault();
			}

			return null;
		}

		public CustomUserPart GetCustomUserByActivationCode(string activationCode)
		{
			return
				this.ContentManager.Query<CustomUserPart, CustomUserRecord>()
					.Where(u => u.ActivationCode == activationCode)
					.List()
					.SingleOrDefault();
		}

		public bool VerifyEmailUnicity(int id, string email)
		{
			var normalizedEmail = email.ToLowerInvariant();
			return this.ContentManager.Query<UserPart, UserPartRecord>().Where(user => user.Email == normalizedEmail).List().All(x => x.Id == id);
		}

		public bool VerifyUsernameUnicity(string username)
		{
			var normalizedUserName = username.ToLowerInvariant();

			return !this.ContentManager.Query<UserPart, UserPartRecord>()
				.Where(user => user.NormalizedUserName == normalizedUserName)
				.List()
				.Any();
		}

		public bool VerifyUsernameUnicity(int id, string userName)
		{
			var normalizedUserName = userName.ToLowerInvariant();

			return this.ContentManager.Query<UserPart, UserPartRecord>()
				.Where(user => user.NormalizedUserName == normalizedUserName)
				.List()
				.All(user => user.Id == id);
		}

		public bool VerifyActivationCodeUnicity(string activationCode)
		{
			var activationCodeExists =
				this.ContentManager.Query<CustomUserPart, CustomUserRecord>(VersionOptions.AllVersions)
					.Where(user => user.ActivationCode != null && user.ActivationCode == activationCode)
					.List()
					.Any();

			return !activationCodeExists;
		}

		public bool VerifyActivationCodeUnicity(int id, string activationCode)
		{
			var activationCodeExists =
				this.ContentManager.Query<CustomUserPart, CustomUserRecord>(VersionOptions.AllVersions)
					.Where(user => user.ActivationCode != null && user.ActivationCode == activationCode)
					.List()
					.Any(user => user.Id != id);

			return !activationCodeExists;
		}
	}
}