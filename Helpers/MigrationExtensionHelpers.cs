namespace Xceno.CustomUsers.Helpers
{
	using System.Globalization;
	using System.Text;
	using Orchard.ContentManagement.MetaData.Builders;

	public static class MigrationExtensionHelpers
	{
		public enum Flavour
		{
			Html,
			Markdown,
			Text,
			Textarea
		}

		public static ContentPartDefinitionBuilder WithMediaLibraryPickerField(
			this ContentPartDefinitionBuilder builder,
			string name,
			bool required = true,
			bool multiple = false,
			string hint = "",
			string displayedContentTypes = "jpg jpeg png gif")
		{
			var displayName = SplitCamel(name);

			// default implementation of Media library picker field - create overloads for more options
			return builder.WithField(
				name,
				fieldBuilder =>
					fieldBuilder.OfType("MediaLibraryPickerField")
						.WithDisplayName(displayName)
						.WithSetting("MediaLibraryPickerFieldSettings.Required", required.ToString(CultureInfo.InvariantCulture))
						.WithSetting("MediaLibraryPickerFieldSettings.DisplayedContentTypes", displayedContentTypes)
						.WithSetting("MediaLibraryPickerFieldSettings.Multiple", multiple.ToString(CultureInfo.InvariantCulture))
						.WithSetting("MediaLibraryPickerFieldSettings.Hint", hint));
		}

		public static ContentPartDefinitionBuilder WithTextField(
			this ContentPartDefinitionBuilder builder,
			string name,
			Flavour flavor,
			bool required = true,
			string hint = "")
		{
			var strFlavor = SplitCamel(flavor.ToString());

			return builder.WithField(
				name,
				fieldBuilder =>
					fieldBuilder.OfType("TextField")
						.WithSetting("TextFieldSettings.Required", required.ToString(CultureInfo.InvariantCulture))
						.WithSetting("TextFieldSettings.Flavor", strFlavor)
						.WithSetting("TextFieldSettings.Hint", hint));
		}

		public static ContentPartDefinitionBuilder WithBooleanField(
			this ContentPartDefinitionBuilder builder,
			string name,
			bool defalut,
			string hint = "")
		{
			return builder.WithField(
				name,
				fieldBuilder =>
					fieldBuilder.OfType("BooleanField")
						.WithSetting("BooleanFieldSettings.Hint", hint)
						.WithSetting("BooleanFieldSettings.DefaultValue", defalut.ToString(CultureInfo.InvariantCulture)));
		}

		public static ContentTypeDefinitionBuilder WithAutoroutePart(
			this ContentTypeDefinitionBuilder builder,
			string pathPrefix = "")
		{
			var pattern = string.Format(
				"[{{Name:'{0}/Title', Pattern: '{0}/{{Content.Slug}}', Description: 'my-page'}}]",
				pathPrefix);

			return builder.WithPart(
				"AutoroutePart",
				partBuilder => partBuilder.WithSetting("AutorouteSettings.PatternDefinitions", pattern));
		}

		public static ContentTypeDefinitionBuilder WithBodyPart(
			this ContentTypeDefinitionBuilder builder,
			Flavour defaultFlavour = Flavour.Html)
		{
			return builder.WithPart(
				"BodyPart",
				partBuilder => partBuilder.WithSetting("BodyTypePartSettings.Flavor", defaultFlavour.ToString()));
		}

		public static ContentTypeDefinitionBuilder WithCommomPart(this ContentTypeDefinitionBuilder builder)
		{
			return builder.WithPart("CommonPart")
				.WithSetting("OwnerEditorSettings.ShowOwnerEditor", false.ToString(CultureInfo.InvariantCulture).ToLower());
		}

		private static string SplitCamel(string enumString)
		{
			var sb = new StringBuilder();
			var last = char.MinValue;
			foreach ( var c in enumString )
			{
				if ( char.IsLower(last) && char.IsUpper(c) )
				{
					sb.Append(' ');
					sb.Append(c.ToString(CultureInfo.InvariantCulture).ToLower());
				}
				else
				{
					sb.Append(c);
				}

				last = c;
			}

			return sb.ToString();
		}
	}
}