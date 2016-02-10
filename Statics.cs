namespace Xceno.CustomUsers
{
	using System;
	using Orchard.DisplayManagement.Shapes;

	public static class Statics
	{
		public const string AngularAppName = "myApp";

		public const string ModuleAreaName = "Xceno.CustomUsers";

		public static OrchardTagBuilder MyAppWrapperTag
		{
			get
			{
				var tag = new OrchardTagBuilder("div");
				tag.MergeAttribute("ng-app", AngularAppName);
				tag.GenerateId("MyAppWrapper");
				return tag;
			}
		}

		public static string AppBasePath
		{
			get { return AppDomain.CurrentDomain.BaseDirectory; }
		}
	}
}