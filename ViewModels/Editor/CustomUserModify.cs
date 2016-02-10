namespace Xceno.CustomUsers.ViewModels.Editor
{
	using System.ComponentModel.DataAnnotations;
	using System.Diagnostics.CodeAnalysis;
	using Models;
	using Services;

	public static class CustomUserFrontendFactory
	{
		/// <summary>
		/// Returns the viewmodel for the angular frontend userProfileEditor
		/// </summary>
		public static CustomUserModify ToViewModel(this CustomUserPart self)
		{
			return new CustomUserModify
					   {
						   Id = self.ContentItem.Id,
						   FirstName = self.FirstName,
						   LastName = self.LastName
					   };
		}

		public static CustomUserPart UpdateFromViewModel(this CustomUserPart self, CustomUserModify viewModel, IMyService myService)
		{
			self.FirstName = viewModel.FirstName;
			self.LastName = viewModel.LastName;
			return self;
		}
	}

	/// <summary>
	/// ViewModel for the front end - UserEditor
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single class", Justification = "Ok")]
	public class CustomUserModify
	{
		/// <summary>
		/// The Id of the customUser
		/// </summary>
		[Required]
		public int Id { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string FirstName { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string LastName { get; set; }

	}
}