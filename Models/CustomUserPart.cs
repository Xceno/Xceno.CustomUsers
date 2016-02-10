namespace Xceno.CustomUsers.Models
{
	using Orchard.ContentManagement;
	using Orchard.ContentManagement.Utilities;
	using Orchard.MediaLibrary.Models;
	using Orchard.Users.Models;

	public class CustomUserPart : ContentPart<CustomUserRecord>
	{
		internal readonly LazyField<UserPart> UserLazyField = new LazyField<UserPart>();
		internal readonly LazyField<MediaPart> ProfilePictureLazyField = new LazyField<MediaPart>();

		public virtual string FirstName
		{
			get { return this.Retrieve(r => r.FirstName); }
			set { this.Store(s => s.FirstName, value); }
		}

		public virtual string LastName
		{
			get { return this.Retrieve(r => r.LastName); }
			set { this.Store(s => s.LastName, value); }
		}

		public virtual string ActivationCode
		{
			get { return this.Retrieve(r => r.ActivationCode); }
			set { this.Store(s => s.ActivationCode, value); }
		}

		public virtual string WelcomeText
		{
			get { return this.Retrieve(r => r.WelcomeText); }
			set { this.Store(s => s.WelcomeText, value); }
		}

		public UserPart User
		{
			get { return this.UserLazyField.Value; }
			set { this.UserLazyField.Value = value; }
		}

		public MediaPart ProfilePicture
		{
			get { return this.ProfilePictureLazyField.Value; }
			set { this.ProfilePictureLazyField.Value = value; }
		}
	}
}