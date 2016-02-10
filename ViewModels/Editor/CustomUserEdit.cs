namespace Xceno.CustomUsers.ViewModels.Editor
{
	using System.ComponentModel.DataAnnotations;
	using Models;
	using Orchard.Users.Models;
	using Services;

	public static class CustomUserAdminFactory
	{
		public static CustomUserPart UpdateFromViewModel(this CustomUserPart self, CustomUserEdit viewModel, IMyService myService)
		{
			self.FirstName = viewModel.FirstName;
			self.LastName = viewModel.LastName;

			return self;
		}
	}

	public class CustomUserEdit
	{
		public CustomUserEdit()
		{
		}

		public CustomUserEdit(CustomUserPart customUser)
		{
			this.Id = customUser.Id;
			this.CanUserDataBeChanged = false;

			if ( customUser.User != null )
			{
				this.OrchardUserId = customUser.User.Id;
				this.UserName = customUser.User.UserName;
				this.Email = customUser.User.Email;

				if ( customUser.User.RegistrationStatus == UserStatus.Pending )
				{
					this.CanUserDataBeChanged = true;
				}
			}

			this.FirstName = customUser.FirstName;
			this.LastName = customUser.LastName;
			this.ActivationCode = customUser.ActivationCode;
			this.WelcomeText = customUser.WelcomeText;
		}

		public int Id { get; set; }

		public int? OrchardUserId { get; private set; }

		[Required]
		public string UserName { get; set; }

		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		public string ActivationCode { get; set; }

		public string WelcomeText { get; set; }

		public bool CanUserDataBeChanged { get; private set; }
	}
}