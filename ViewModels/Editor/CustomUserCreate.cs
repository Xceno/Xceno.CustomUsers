namespace Xceno.CustomUsers.ViewModels.Editor
{
	using System.ComponentModel.DataAnnotations;

	public class CustomUserCreate
	{
		[Required]
		public string UserName { get; set; }

		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		public string Email { get; set; }

		[Required]
		public string ActivationCode { get; set; }

		public string WelcomeText { get; set; }
	}
}