namespace Xceno.CustomUsers.Activities
{
	using Orchard.Localization;

	public class MyCustomUserRequestedInviteActivity : MyCustomActivity
	{
		public override string Name
		{
			get { return "CustomUserRequestedInvite"; }
		}

		public override LocalizedString Description
		{
			get { return this.T("An anonymous user requested an invite to the platform"); }
		}
	}
}