namespace Xceno.CustomUsers.Handlers
{
	using Models;
	using Orchard.ContentManagement;
	using Orchard.ContentManagement.Handlers;
	using Orchard.Localization;

	public class CustomSettingsPartHandler : ContentHandler
	{
		public CustomSettingsPartHandler()
		{
			this.Filters.Add(new ActivatingFilter<CustomSettingsPart>("Site"));
			this.Filters.Add(new TemplateFilterForPart<CustomSettingsPart>("CustomSettings", "Parts/Custom.Settings", "Custom"));

			this.T = NullLocalizer.Instance;
		}

		public Localizer T { get; set; }

		protected override void GetItemMetadata(GetContentItemMetadataContext context)
		{
			if ( context.ContentItem.ContentType != "Site" )
				return;
			base.GetItemMetadata(context);
			context.Metadata.EditorGroupInfo.Add(new GroupInfo(this.T("Custom")));
		}
	}
}