namespace Xceno.CustomUsers.Controllers
{
	using System;
	using System.Text.RegularExpressions;
	using System.Web.Mvc;
	using Orchard;
	using Orchard.ContentManagement;
	using Orchard.Core.Settings.Models;
	using Orchard.DisplayManagement;
	using Orchard.Localization;
	using Orchard.Settings;
	using Orchard.UI.Admin;
	using Orchard.UI.Notify;
	using Orchard.Users;
	using Orchard.Users.Models;
	using Orchard.Users.Services;
	using ViewModels.Editor;
	using Events;
	using Models;
	using Services;

	[Admin]
	public class CustomUserAdminController : BaseMcvController, IUpdateModel
	{
		private readonly IContentManager contentManager;
		private readonly ISiteService siteService;
		private readonly IUserService userService;

		public CustomUserAdminController(
			IMyService myService,
			IShapeFactory shapeFactory,
			ISiteService siteService,
			IContentManager contentManager,
			IOrchardServices orchardServices,
			IUserService userService,
			ICustomUserEventHandler customUserEventHandler)
			: base(myService, customUserEventHandler, orchardServices)
		{
			this.Shape = shapeFactory;
			this.T = NullLocalizer.Instance;
			this.siteService = siteService;
			this.contentManager = contentManager;
			this.userService = userService;
		}

		public dynamic Shape { get; set; }

		public Localizer T { get; set; }

		bool IUpdateModel.TryUpdateModel<TModel>(
			TModel model,
			string prefix,
			string[] includeProperties,
			string[] excludeProperties)
		{
			return this.TryUpdateModel(model, prefix, includeProperties, excludeProperties);
		}

		void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
		{
			this.ModelState.AddModelError(key, errorMessage.Text);
		}

		public ActionResult Index()
		{
			return this.View();
		}

		public ActionResult Create()
		{
			if ( !this.OrchardServices.Authorizer.Authorize(Permissions.ManageUsers, this.T("Not authorized to manage users")) )
			{
				return new HttpUnauthorizedResult();
			}

			var user = this.contentManager.New("CustomUser");
			var editor = this.Shape.EditorTemplate(
				TemplateName: "Parts/CustomUser.Create",
				Model: new CustomUserCreate(),
				Prefix: null);
			editor.Metadata.Position = "2";
			var model = this.OrchardServices.ContentManager.BuildEditor(user);
			model.Content.Add(editor);

			return this.View((object)model);
		}

		[HttpPost]
		[ActionName("Create")]
		public ActionResult CreatePost(CustomUserCreate createModel)
		{
			if ( !this.OrchardServices.Authorizer.Authorize(Permissions.ManageUsers, this.T("Not authorized to manage users")) )
			{
				return new HttpUnauthorizedResult();
			}

			if ( !string.IsNullOrEmpty(createModel.UserName) )
			{
				if ( !this.MyService.VerifyUsernameUnicity(createModel.UserName) )
				{
					this.ModelState.AddModelError(
						"NotUniqueUserName",
						this.T("CustomUser with that username already exists.").ToString());
				}
			}

			if ( !this.MyService.VerifyActivationCodeUnicity(createModel.ActivationCode) )
			{
				this.ModelState.AddModelError("NotUniqueActivationCode", this.T("This Activation Code is already in use").ToString());
			}

			if ( !this.ModelState.IsValid )
			{
				this.OrchardServices.TransactionManager.Cancel();

				var editor = this.Shape.EditorTemplate(TemplateName: "Parts/CustomUser.Create", Model: createModel, Prefix: null);
				editor.Metadata.Position = "2";
				var customUser = this.OrchardServices.ContentManager.New<CustomUserPart>("CustomUser");
				var model = this.OrchardServices.ContentManager.UpdateEditor(customUser, this);
				model.Content.Add(editor);

				return this.View(model);
			}

			if ( this.ModelState.IsValid )
			{
				this.MyService.CreateCustomUser(createModel);
			}

			this.OrchardServices.Notifier.Information(this.T("CustomUser created"));
			return this.RedirectToAction("Index");
		}

		public ActionResult Edit(int id)
		{
			if ( !this.OrchardServices.Authorizer.Authorize(Permissions.ManageUsers, this.T("Not authorized to manage users")) )
			{
				return new HttpUnauthorizedResult();
			}

			var customUser = this.MyService.GetCustomUser(id);
			var editor = this.Shape.EditorTemplate(
				TemplateName: "Parts/CustomUser.Edit",
				Model: new CustomUserEdit(customUser),
				Prefix: null);
			editor.Metadata.Position = "2";
			var model = this.OrchardServices.ContentManager.BuildEditor(customUser);
			model.Content.Add(editor);

			return this.View((object)model);
		}

		[HttpPost]
		[ActionName("Edit")]
		public ActionResult EditPost(int id)
		{
			if ( !this.OrchardServices.Authorizer.Authorize(Permissions.ManageUsers, this.T("Not authorized to manage users")) )
			{
				return new HttpUnauthorizedResult();
			}

			var customUser = this.OrchardServices.ContentManager.Get<CustomUserPart>(id, VersionOptions.DraftRequired);
			var previousName = customUser.User.UserName;

			var editModel = new CustomUserEdit(customUser);

			if ( this.TryUpdateModel(editModel) )
			{
				if ( customUser.User.RegistrationStatus == UserStatus.Approved )
				{
					if ( string.IsNullOrWhiteSpace(editModel.Email) )
					{
						this.ModelState.AddModelError("Email", this.T("Active Users need a valid email adress").ToString());
					}

					if ( !this.userService.VerifyUserUnicity(id, editModel.UserName, editModel.Email) )
					{
						this.ModelState.AddModelError(
							"NotUniqueUserName",
							this.T("CustomUser with that username and/or email already exists.").ToString());
					}
					else if ( !Regex.IsMatch(editModel.Email ?? string.Empty, UserPart.EmailPattern, RegexOptions.IgnoreCase) )
					{
						// http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx
						this.ModelState.AddModelError("Email", this.T("You must specify a valid email address.").ToString());
					}
				}
				else
				{
					if ( !this.MyService.VerifyUsernameUnicity(id, editModel.UserName) )
					{
						this.ModelState.AddModelError(
							"NotUniqueUserName",
							this.T("CustomUser with that username already exists.").ToString());
					}
				}

				if ( !this.MyService.VerifyActivationCodeUnicity(id, editModel.ActivationCode) )
				{
					this.ModelState.AddModelError("NotUniqueActivationCode", this.T("This Activation Code is already in use").ToString());
				}

				if ( this.ModelState.IsValid )
				{
					// also update the Super user if this is the renamed account
					if ( string.Equals(this.OrchardServices.WorkContext.CurrentSite.SuperUser, previousName, StringComparison.Ordinal) )
					{
						this.siteService.GetSiteSettings().As<SiteSettingsPart>().SuperUser = editModel.UserName;
					}

					customUser.User.NormalizedUserName = editModel.UserName.ToLowerInvariant();
				}
			}

			if ( !this.ModelState.IsValid )
			{
				this.OrchardServices.TransactionManager.Cancel();

				var editor = this.Shape.EditorTemplate(TemplateName: "Parts/CustomUser.Edit", Model: editModel, Prefix: null);
				editor.Metadata.Position = "2";
				var model = this.OrchardServices.ContentManager.UpdateEditor(customUser, this);
				model.Content.Add(editor);

				return this.View((object)model);
			}

			this.OrchardServices.ContentManager.Publish(customUser.ContentItem);

			this.OrchardServices.Notifier.Information(this.T("CustomUser information updated"));
			return this.RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult Delete(int customUserId)
		{
			if ( !this.OrchardServices.Authorizer.Authorize(Permissions.ManageUsers, this.T("Not authorized to manage users")) )
			{
				return new HttpUnauthorizedResult();
			}

			this.MyService.DeleteCustomUser(customUserId);
			this.OrchardServices.Notifier.Information(this.T("User was deleted"));

			return this.RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult RetriggerCreatedEvent(int id)
		{
			if ( !this.OrchardServices.Authorizer.Authorize(Permissions.ManageUsers, this.T("Not authorized to manage users")) )
			{
				return new HttpUnauthorizedResult();
			}

			var customUser = this.MyService.GetCustomUser(id);
			this.CustomUserEventHandler.CustomUserCreated(customUser);

			this.OrchardServices.Notifier.Information(this.T("The event was retriggered for {0}", customUser.LastName));

			return this.RedirectToAction("Index");
		}
	}
}