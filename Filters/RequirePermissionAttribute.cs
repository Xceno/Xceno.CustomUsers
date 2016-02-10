namespace Xceno.CustomUsers.Filters
{
	using System;
	using System.Linq;
	using System.Web.Http;
	using System.Web.Http.Controllers;
	using Orchard;
	using Orchard.Security;
	using Orchard.Security.Permissions;

	/// <summary>
	///     Filter to require the specified Orchard permission(s) for the decorated WebApi controller/action.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class RequirePermissionsAttribute : AuthorizeAttribute
	{
		private readonly string[] permissionNames;

		public RequirePermissionsAttribute(params string[] permissionNames)
		{
			this.permissionNames = permissionNames;
		}

		protected override bool IsAuthorized(HttpActionContext actionContext)
		{
			if ( actionContext == null )
			{
				throw new ArgumentNullException("actionContext");
			}

			var workContext = actionContext.ControllerContext.GetWorkContext();
			var user = workContext.CurrentUser;
			if ( user == null )
			{
				return false;
			}

			var permissions = this.permissionNames.Select(name => new Permission { Name = name });
			var authorizer = workContext.Resolve<IAuthorizer>();

			foreach ( var permission in permissions )
			{
				if ( !authorizer.Authorize(permission) )
				{
					return false;
				}
			}

			return true;
		}
	}
}