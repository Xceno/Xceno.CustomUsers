namespace Xceno.CustomUsers.Activities
{
	using Orchard.Localization;

	public class MyCustomUserActivatedActivity : MyCustomActivity
	{
		public override string Name
		{
			get { return "CustomUserActivated"; }
		}

		public override LocalizedString Description
		{
			get { return this.T("A pending CustomUser was activated"); }
		}
	}
}