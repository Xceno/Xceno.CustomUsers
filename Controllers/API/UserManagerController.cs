namespace Xceno.CustomUsers.Controllers.API
{
	using Events;
	using Filters;
	using Orchard;
	using Services;

	[RequireBasicAuthentication]
	public class UserManagerController : BaseApiController
	{
		public UserManagerController(
			IMyService myService,
			ICustomUserEventHandler customUserEventHandler,
			IOrchardServices orchardServices)
			: base(myService, customUserEventHandler, orchardServices)
		{
		}

		// You can use this controller for a custom admin-view for your users in the orchard dashboard.
		// We use AngularJS for all our Views.
	}
}