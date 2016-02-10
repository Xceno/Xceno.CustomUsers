namespace Xceno.CustomUsers
{
	using System.Web.Routing;
	using Orchard.Environment;
	using Orchard.Localization;
	using Orchard.UI.Navigation;

	public class AdminMenu : INavigationProvider
	{
		public AdminMenu(Work<RequestContext> requestContextAccessor)
		{
			this.T = NullLocalizer.Instance;
		}

		public Localizer T { get; set; }

		public string MenuName
		{
			get { return "admin"; }
		}

		public void GetNavigation(NavigationBuilder builder)
		{
			builder
				.AddImageSet("CustomUsers")
				.Add(
					this.T("CustomUsers"),
					"2",
					itemBuilder => itemBuilder
						.LinkToFirstChild(false)
						.Add(this.T("Users"), "2", item => item.Action("Index", "CustomUserAdmin", new { area = Statics.ModuleAreaName })));
		}
	}
}