namespace Xceno.CustomUsers.Models
{
	using Orchard.ContentManagement;

	public class CustomSettingsPart : ContentPart
	{
		public bool CustomUserActivationApprovesOrchardUser
		{
			get { return this.Retrieve(r => r.CustomUserActivationApprovesOrchardUser); }
			set { this.Store(s => s.CustomUserActivationApprovesOrchardUser, value); }
		}
	}
}