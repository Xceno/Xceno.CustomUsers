namespace Xceno.CustomUsers.Tokens
{
	using Models;
	using Orchard.ContentManagement;
	using Orchard.Localization;
	using Orchard.Tokens;

	public class CustomUserTokens : ITokenProvider
	{
		public CustomUserTokens()
		{
			this.T = NullLocalizer.Instance;
		}

		public Localizer T { get; set; }

		public void Describe(DescribeContext context)
		{
			context.For("CustomUser", this.T("Custom User"), this.T("Tokens for custom users"))
				.Token("FullName", this.T("Full name"), this.T("The full name of the user"))
				.Token("NameAffix", this.T("Name affix"), this.T("The CustomUser name affix"))
				.Token("ActivationCode", this.T("Activation code"), this.T("The CustomUser activation code"))
				.Token("WelcomeText", this.T("Welcome text"), this.T("The custom welcome text for the CustomUser"))
				.Token("User", this.T("The Orchard User"), this.T("Gets the orchard user from the CustomUser"));
		}

		public void Evaluate(EvaluateContext context)
		{
			context.For<IContent>("CustomUser")
				.Token("ActivationCode", content => content.As<CustomUserPart>().ActivationCode)
				.Chain("ActivationCode", "ActivationCode", content => content.As<CustomUserPart>().ActivationCode)
				.Token("WelcomeText", content => content.As<CustomUserPart>().WelcomeText)
				.Chain("WelcomeText", "WelcomeText", content => content.As<CustomUserPart>().WelcomeText)
				.Token("User", content => content.As<CustomUserPart>().User)
				.Chain("User", "User", content => content.As<CustomUserPart>().User);
		}
	}
}