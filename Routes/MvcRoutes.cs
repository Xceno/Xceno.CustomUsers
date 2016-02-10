namespace Xceno.CustomUsers.Routes
{
	using System.Collections.Generic;
	using System.Web.Mvc;
	using System.Web.Routing;
	using Orchard.Mvc.Routes;
	using CustomUsers;

	public class MvcRoutes : IRouteProvider
	{
		public IEnumerable<RouteDescriptor> GetRoutes()
		{
			return new[]
				{
				// Admin - Custom User
				BuildRoute("CustomUserAdmin", "Index", "Admin/Custom/Users"),
				BuildRoute("CustomUserAdmin", "ContactIndex", "Admin/Custom/Users/{id}/contacts"),
				BuildRoute("CustomUserAdmin", "Create", "Admin/Contents/Create/CustomUser"),
				BuildRoute("CustomUserAdmin", "Delete", "Admin/Contents/Delete/CustomUser/{customUserId}"),
				BuildRoute("CustomUserAdmin", "Index", "Admin/Custom/Users/{id}/Edit"), // Index is the wrapper for angular. This is not a typo! ;)
				BuildRoute("CustomUserAdmin", "Index", "Admin/Custom/Users/Create"), // Index is the wrapper for angular. This is not a typo! ;)
				BuildRoute("CustomUserAdmin", "GetDownloadUserImages", "Admin/Custom/Users/UserImages"),

				// Public - Users
				BuildRoute("Users", "Index", "custom/users/{*.}"),
				};
		}

		public void GetRoutes(ICollection<RouteDescriptor> routes)
		{
			foreach ( var routeDescriptor in this.GetRoutes() )
			{
				routes.Add(routeDescriptor);
			}
		}

		private static RouteDescriptor BuildRoute(string controller, string action, string url, int priority = 5)
		{
			return new RouteDescriptor
			{
				Priority = priority,
				Route = new Route(
					url,
					new RouteValueDictionary
						{
						{ "area", Statics.ModuleAreaName },
						{ "controller", controller },
						{ "action", action }
						},
					new RouteValueDictionary(),
					new RouteValueDictionary { { "area", Statics.ModuleAreaName } },
					new MvcRouteHandler())
			};
		}
	}
}