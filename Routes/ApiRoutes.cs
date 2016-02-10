namespace Xceno.CustomUsers.Routes
{
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Web.Http.Routing;
	using Orchard.Mvc.Routes;
	using Orchard.WebApi.Routes;

	public class ApiRoutes : IHttpRouteProvider
	{
		private const string BaseApiRoute = "api/xc/";

		public IEnumerable<RouteDescriptor> GetRoutes()
		{
			////return new[]
			////	   {
			////		   // UserManagerController
			////		   MakeRouteDescriptor(typeof(UserManagerController), "usermanager/users", "GetAll"),
			////		   MakeRouteDescriptor(typeof(UserManagerController), "usermanager/users", "Post"),
			////		   MakeRouteDescriptor(typeof(UserManagerController), "usermanager/users/{userId}", "Get"),
			////		   MakeRouteDescriptor(typeof(UserManagerController), "usermanager/users/{userId}", "Put"),
			////	   };
			
			return new RouteDescriptor[0];
		}

		public void GetRoutes(ICollection<RouteDescriptor> routes)
		{
			foreach ( var routeDescriptor in this.GetRoutes() )
			{
				routes.Add(routeDescriptor);
			}
		}

		/// <summary>
		/// Creates a new RouteDescriptor in the style of attributeRouting. Uses the module as default area
		/// </summary>
		/// <param name="controller">The type of the controller</param>
		/// <param name="routeSlug">The route of the action. Will be appended to the base api route.</param>
		/// <param name="action">
		/// The name of the action.
		/// <remarks>Prefix it with the desired httpVerb for the action</remarks>
		/// <example>GetUser, PostUser, PutUser</example>
		/// </param>
		/// <returns>A fully configured HttpRouteDescriptor</returns>
		private static HttpRouteDescriptor MakeRouteDescriptor(Type controller, string routeSlug, string action)
		{
			var httpMethod = HttpMethod.Get;
			if ( action.StartsWith(HttpMethod.Post.Method, StringComparison.InvariantCultureIgnoreCase) )
			{
				httpMethod = HttpMethod.Post;
			}
			else if ( action.StartsWith(HttpMethod.Put.Method, StringComparison.InvariantCultureIgnoreCase) )
			{
				httpMethod = HttpMethod.Put;
			}
			else if ( action.StartsWith(HttpMethod.Delete.Method, StringComparison.InvariantCultureIgnoreCase) )
			{
				httpMethod = HttpMethod.Delete;
			}

			var route = BaseApiRoute + routeSlug;
			var ctrlIndex = controller.Name.LastIndexOf("Controller", StringComparison.Ordinal);
			var controllerName = controller.Name.Substring(0, ctrlIndex);

			return MakeRouteDescriptor(route, controllerName, action, httpMethod: httpMethod);
		}

		/// <summary>
		/// Erstellt einen neuen RouteDescriptor mit dem Modul als default-area.
		/// Nur dafür da um GetRoutes() lesbarer zu machen
		/// </summary>
		private static HttpRouteDescriptor MakeRouteDescriptor(string route, string controller, string action, int priority = 5, HttpMethod httpMethod = null)
		{
			var routeDescriptor = new HttpRouteDescriptor
			{
				RouteTemplate = route,
				Defaults = new { area = Statics.ModuleAreaName, controller, action },
				Priority = priority
			};

			if ( httpMethod != null )
			{
				routeDescriptor.Constraints = new { httpMethod = new HttpMethodConstraint(httpMethod) };
			}

			return routeDescriptor;
		}
	}
}