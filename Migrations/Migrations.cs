namespace Xceno.CustomUsers.Migrations
{
	using System;
	using Models;
	using Orchard.ContentManagement;
	using Orchard.Data.Migration;
	using Orchard.Localization;
	using Orchard.Security;
	using Orchard.Settings;
	using Orchard.Users.Models;

	// We split up the migration into several partials.
	// This is really handy if you have a lot of different ContentTypes, etc.
	// In this example it's totaly overengineered, but it brings the point accross I guess...
	public partial class Migrations : DataMigrationImpl
	{
		private readonly IContentManager contentManager;
		private readonly IMembershipService membershipService;
		private readonly ISiteService siteService;

		private IUser superUser;

		public Migrations(
			IContentManager contentManager,
			ISiteService siteService,
			IMembershipService membershipService)
		{
			this.contentManager = contentManager;
			this.siteService = siteService;
			this.membershipService = membershipService;
			this.T = NullLocalizer.Instance;
		}

		public Localizer T { get; set; }

		public int Create()
		{
			// base setup
			this.RunAllUserMigrations();

			// Creating an account for the sites superUser
			var username = this.siteService.GetSiteSettings().SuperUser;
			this.superUser = this.membershipService.GetUser(username);
			var customUserPart = this.contentManager.New<CustomUserPart>("CustomUser");
			customUserPart.FirstName = "Administrator";
			customUserPart.LastName = "Administrator";
			customUserPart.User = this.superUser.ContentItem.As<UserPart>();
			this.contentManager.Create(customUserPart);

			return 15;
		}

		/// <summary>Creates a foreignkey based on some defaults</summary>
		/// <param name="srcRecordType">The type of the source contentTypeRecord, which will resolve to the source table</param>
		/// <param name="destRecordType">The type of the destination contentTypeRecord, which will resolve to the destination table</param>
		/// <param name="srcColumn">Overrides the source column name. Defaults to 'DestinationContentTypeRecord_Id'</param>
		/// <param name="destColumn">Overrides the destination column name</param>
		/// <param name="destModule">Optional. Specify the full module name if the destination table is from another module</param>
		private void CreateForeignKey(Type srcRecordType, Type destRecordType, string srcColumn = null, string destColumn = "Id", string destModule = null)
		{
			var foreignKeyName = this.CreateForeignKeyName(srcRecordType, destRecordType, srcColumn, destColumn, destModule);
			var srcTableName = srcRecordType.Name;
			var destTableName = destRecordType.Name;
			srcColumn = string.IsNullOrEmpty(srcColumn) ? string.Format("{0}_Id", destTableName) : srcColumn;

			if ( !string.IsNullOrEmpty(destModule) )
			{
				this.SchemaBuilder.CreateForeignKey(
					foreignKeyName,
					srcTableName,
					new[] { srcColumn },
					destModule,
					destTableName,
					new[] { destColumn });
			}
			else
			{
				this.SchemaBuilder.CreateForeignKey(
					foreignKeyName,
					srcTableName,
					new[] { srcColumn },
					destTableName,
					new[] { destColumn });
			}
		}

		private string CreateForeignKeyName(Type srcRecordType, Type destRecordType, string srcColumn = null, string destColumn = "Id", string destModule = null)
		{
			var srcTableName = srcRecordType.Name;
			var destTableName = destRecordType.Name;
			srcColumn = string.IsNullOrEmpty(srcColumn) ? string.Format("{0}_Id", destTableName) : srcColumn;

			var foreignKeyName = string.Format("FK__{0}{1}_{2}{3}", srcTableName, srcColumn, destTableName, destColumn != "Id" ? destColumn : string.Empty);
			return foreignKeyName;
		}
	}
}