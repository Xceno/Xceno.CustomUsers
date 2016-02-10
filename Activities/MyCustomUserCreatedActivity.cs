namespace Xceno.CustomUsers.Activities
{
	using Orchard.Localization;

	public class MyCustomUserCreatedActivity : MyCustomActivity
	{
		public override string Name
		{
			get { return "CustomUserCreated"; }
		}

		public override LocalizedString Description
		{
			get { return this.T("A new CustomUser was created"); }
		}
	}
}