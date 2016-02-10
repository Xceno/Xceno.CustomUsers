namespace Xceno.CustomUsers.Models
{
	using Orchard.ContentManagement.Records;
	using Orchard.Users.Models;

	public class CustomUserRecord : ContentPartRecord
	{
		public virtual string FirstName { get; set; }

		public virtual string LastName { get; set; }

		public virtual string ActivationCode { get; set; }

		public virtual string WelcomeText { get; set; }

		public virtual UserPartRecord UserPartRecord { get; set; }
	}
}