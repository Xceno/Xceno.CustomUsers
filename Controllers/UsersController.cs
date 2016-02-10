namespace Xceno.CustomUsers.Controllers
{
	using System.Diagnostics;
	using System.Text.RegularExpressions;
	using System.Web.Mvc;
	using Models;
	using Orchard.Localization;
	using Orchard.Security;
	using Orchard.Themes;
	using Orchard.UI.Notify;
	using Orchard.Users.Models;
	using Services;
	using ViewModels;

	[Themed]
	public class UsersController : Controller
	{
		private readonly IMyService myService;
		private readonly INotifier notifier;

		public UsersController(INotifier notifier, IMyService myService)
		{
			this.notifier = notifier;
			this.myService = myService;
			this.T = NullLocalizer.Instance;
		}

		public Localizer T { get; set; }

		// GET: Users
		[Authorize]
		public ActionResult Index()
		{
			return this.View();
		}

		[AllowAnonymous]
		[HttpGet]
		public ActionResult Activate(string activationCode)
		{
			if ( string.IsNullOrWhiteSpace(activationCode) )
			{
				this.notifier.Add(NotifyType.Warning, this.T("You cannot activate without an activation code!"));
				return this.View("LogOn");
			}

			var userFromActivationCode = this.myService.GetCustomUserByActivationCode(activationCode);

			if ( userFromActivationCode == null )
			{
				this.notifier.Add(NotifyType.Error, this.T("Unkown activation code!"));
				return this.View("LogOn");
			}

			if ( userFromActivationCode.User.RegistrationStatus == UserStatus.Approved )
			{
				this.notifier.Add(NotifyType.Error, this.T("This account is already active"));
				return this.View("LogOn");
			}

			var viewModel = new CustomUserActivate
			{
				ActivationCode = userFromActivationCode.ActivationCode,
				UserName = userFromActivationCode.User.UserName,
				WelcomeText = userFromActivationCode.WelcomeText,
				Email = userFromActivationCode.User.Email
			};

			return this.View(viewModel);
		}

		[AllowAnonymous]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Activate(CustomUserActivate input)
		{
			if ( input == null )
			{
				this.ModelState.AddModelError("_form", this.T("The argument cannot be null").Text);
			}

			CustomUserPart customUserPart = null;
			if ( this.ModelState.IsValid )
			{
				customUserPart = this.myService.GetCustomUserByActivationCode(input.ActivationCode);

				if ( customUserPart == null || customUserPart.User == null || customUserPart.User.UserName != input.UserName )
				{
					this.notifier.Add(NotifyType.Error, this.T("The activation failed"));
				}

				if ( string.IsNullOrEmpty(input.Email) )
				{
					this.ModelState.AddModelError("Email", this.T("You must specify an email address.").Text);
				}
				else if ( input.Email.Length >= 255 )
				{
					this.ModelState.AddModelError("Email", this.T("The email address you provided is too long.").Text);
				}
				else if ( !Regex.IsMatch(input.Email, UserPart.EmailPattern, RegexOptions.IgnoreCase) )
				{
					// http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx    
					this.ModelState.AddModelError("Email", this.T("You must specify a valid email address.").Text);
				}
				else if ( !this.myService.VerifyEmailUnicity(customUserPart.User.Id, input.Email) )
				{
					this.ModelState.AddModelError("Email", this.T("This email address is already in use.").Text);
				}
			}

			if ( !this.ModelState.IsValid )
			{
				return this.View(input);
			}

			Debug.Assert(customUserPart != null, "customUserPart != null");
			var user = customUserPart.User;
			var userParams = new CreateUserParams(user.UserName, input.Password, input.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true);
			this.myService.ActivateCustomUser(customUserPart.Id, userParams);

			this.notifier.Add(NotifyType.Information, this.T("Your account was activated. You can now log in."));
			return this.RedirectToAction("LogOn", "Account", new { area = "Orchard.Users" });
		}
	}
}