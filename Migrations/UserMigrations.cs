namespace Xceno.CustomUsers.Migrations
{
	using Orchard.ContentManagement.MetaData;
	using Orchard.Core.Contents.Extensions;
	using Orchard.Users.Models;
	using Models;

	public partial class Migrations
	{
		/// <summary>
		/// Creates everything necessary for a basic custom user
		/// </summary>
		private void RunAllUserMigrations()
		{
			this.RunCustomUserMigration();

			// ForeignKey Setup
			this.CreateForeignKey(typeof(CustomUserRecord), typeof(UserPartRecord), destModule: "Orchard.Users");
		}

		/// <summary>
		/// Creates the CustomUserRecord table, the CustomUser ContentPart and the CustomUser ContentType
		/// </summary>
		private void RunCustomUserMigration()
		{
			this.SchemaBuilder.CreateTable(
				typeof(CustomUserRecord).Name,
				table =>
				table.ContentPartRecord()
					.Column<int>("UserPartRecord_Id")
					.Column<string>("FirstName")
					.Column<string>("LastName")
					.Column<string>("ActivationCode", c => c.WithLength(500))
					.Column<string>("WelcomeText", c => c.WithLength(3000)));

			this.ContentDefinitionManager.AlterPartDefinition("CustomUser", builder => builder.WithDescription("Turns content types into a CustomUser."));
			this.ContentDefinitionManager.AlterTypeDefinition("CustomUser", builder => builder.Creatable().Listable(false));
		}
	}
}