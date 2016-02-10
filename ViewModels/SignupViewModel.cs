namespace Xceno.CustomUsers.ViewModels
{
	using System.ComponentModel.DataAnnotations;

	public class SignupViewModel
	{
		[StringLength(255), Required, DataType(DataType.EmailAddress), Display(Name = "Email")]
		public string Email { get; set; }

		[StringLength(255), Required, DataType(DataType.Password), Display(Name = "Password")]
		public string Password { get; set; }

		[StringLength(255), Required, DataType(DataType.Password), Compare("Password"), Display(Name = "Repeat password")]
		public string RepeatPassword { get; set; }
	}
}
