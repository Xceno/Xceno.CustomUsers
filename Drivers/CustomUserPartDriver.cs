namespace Xceno.CustomUsers.Drivers
{
	using Models;
	using Orchard.ContentManagement;
	using Orchard.ContentManagement.Drivers;
	using Orchard.Localization;
	using Orchard.UI.Notify;

	public class CustomUserPartDriver : ContentPartDriver<CustomUserPart>
	{
		private const string TemplateName = "Parts/CustomUser";

		private const string ShapeType = "Parts_CustomUser";

		private readonly INotifier notifier;

		public CustomUserPartDriver(INotifier notifier)
		{
			this.notifier = notifier;
			this.T = NullLocalizer.Instance;
		}

		public Localizer T { get; set; }

		protected override DriverResult Display(CustomUserPart part, string displayType, dynamic shapeHelper)
		{
			return this.ContentShape(ShapeType, () => shapeHelper.Parts_CustomUser(ContentItem: part.ContentItem));
		}

		protected override DriverResult Editor(CustomUserPart part, dynamic shapeHelper)
		{
			return this.ContentShape(ShapeType, () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: this.Prefix));
		}

		protected override DriverResult Editor(CustomUserPart part, IUpdateModel updater, dynamic shapeHelper)
		{
			if ( updater.TryUpdateModel(part, this.Prefix, null, null) )
			{
				this.notifier.Information(this.T("CustomUserPart edited successfully"));
			}
			else
			{
				this.notifier.Error(this.T("Error during CustomUserPart update!"));
			}

			if ( updater.TryUpdateModel(part.User, this.Prefix, new[] { "Email", "UserName" }, null) )
			{
				this.notifier.Information(this.T("CustomUserPart.User edited successfully"));
			}
			else
			{
				this.notifier.Error(this.T("Error during CustomUserPart.User update!"));
			}

			return this.Editor(part, shapeHelper);
		}
	}
}