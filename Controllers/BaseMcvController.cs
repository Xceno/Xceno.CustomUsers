namespace Xceno.CustomUsers.Controllers
{
	using System;
	using System.Web.Mvc;
	using Orchard;
	using Orchard.ContentManagement;
	using Orchard.Security;
	using Orchard.Users.Models;
	using Events;
	using Models;
	using Services;

	public abstract class BaseMcvController : Controller
	{
		internal readonly IMyService MyService;
		internal readonly ICustomUserEventHandler CustomUserEventHandler;
		internal readonly IOrchardServices OrchardServices;

		public BaseMcvController(IMyService myService, ICustomUserEventHandler customUserEventHandler, IOrchardServices orchardServices)
		{
			this.MyService = myService;
			this.CustomUserEventHandler = customUserEventHandler;
			this.OrchardServices = orchardServices;
		}

		/// <summary>
		/// The current user from the Orchard WorkContext
		/// </summary>
		protected IUser CurrentUser
		{
			get { return this.OrchardServices.WorkContext.CurrentUser; }
		}

		/// <summary>
		/// The customUserPart for the current user<br />
		/// ATTENTION: If a super user or admin without an existing customUserPart is accessing this property,
		/// a new customUserPart will be created on the fly with the current super/admin user as attachment.
		/// </summary>
		protected CustomUserPart CurrentCustomUser
		{
			get
			{
				var customUser = this.MyService.GetCustomUser(this.CurrentUser.ContentItem.Id);
				if ( customUser != null )
				{
					return customUser;
				}

				if ( this.CurrentUserCanAccessAdminPanel || this.CurrentUserIsSiteOwner )
				{
					var user = this.CurrentUser;
					customUser = this.OrchardServices.ContentManager.New<CustomUserPart>("CustomUser");
					customUser.FirstName = user.UserName;
					customUser.LastName = user.UserName;
					customUser.User = user.ContentItem.As<UserPart>();
					this.OrchardServices.ContentManager.Create(customUser.ContentItem);
					return customUser;
				}

				return null;
			}
		}

		/// <summary>
		/// Uses the Orchard Authorizer to check the current user for the "AccessAdminPanel" permission
		/// </summary>
		protected bool CurrentUserCanAccessAdminPanel
		{
			get { return this.OrchardServices.Authorizer.Authorize(StandardPermissions.AccessAdminPanel); }
		}

		/// <summary>
		/// Uses the Orchard Authorizer to check if the current user is the site owner
		/// </summary>
		protected bool CurrentUserIsSiteOwner
		{
			get { return this.OrchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner); }
		}

	}
}