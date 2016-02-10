namespace Xceno.CustomUsers.Services
{
	using System.Collections.Generic;
	using Models;
	using Orchard;
	using Orchard.Security;
	using ViewModels.Editor;

	public interface IMyService : IDependency
	{
		/// <summary>
		/// Creates a InsideUser and does a simple registration for the attached UserPart with pending status.
		/// </summary>
		CustomUserPart CreateCustomUser(CustomUserCreate customUserCreate);

		/// <summary>
		/// Activates the given custom user and completes the registrations procedure for the attached UserPart
		/// </summary>
		/// <returns>The updated custom user</returns>
		CustomUserPart ActivateCustomUser(int userId, CreateUserParams createUserParams);

		/// <summary>
		/// Deletes a custom user and all it's metadata, as well as the underlying orchard user
		/// </summary>
		/// <param name="customUserId">The Id of the custom user to delete</param>
		void DeleteCustomUser(int customUserId);

		/// <summary>
		/// Get all CustomUsers
		/// </summary>
		/// <returns>A list of custom users</returns>
		IEnumerable<CustomUserPart> GetCustomUsers();

		/// <summary>
		/// Get a custom user by its Id
		/// </summary>
		/// <returns>The found custom user or null if it couldn't be found</returns>
		CustomUserPart GetCustomUser(int id);

		/// <summary>
		/// Get a CustomUser by its activationCode
		/// </summary>
		/// <returns>The found custom user or null</returns>
		CustomUserPart GetCustomUserByActivationCode(string activationCode);

		/// <summary>
		/// Checks if a user with a given email address already exusts
		/// </summary>
		/// <returns>true if the email address is unique</returns>
		bool VerifyEmailUnicity(int id, string email);

		/// <summary>
		/// Checks if a user with a given username already exists.
		/// </summary>
		/// <returns>true if the username is unique</returns>
		bool VerifyUsernameUnicity(string username);

		/// <summary>
		/// Checks if a user with a given username already exists, ignoring the user with the given Id.
		/// </summary>
		/// <returns>true if the username is unique</returns>
		bool VerifyUsernameUnicity(int id, string userName);

		/// <summary>
		/// Checks if the given activationCode already exists.
		/// </summary>
		/// <returns>true if the activationCode is unique</returns>
		bool VerifyActivationCodeUnicity(string activationCode);

		/// <summary>
		/// Checks if the given activationCode already exists, ignoring the user with the given Id.
		/// </summary>
		/// <returns>true if the activationCode is unique</returns>
		bool VerifyActivationCodeUnicity(int id, string activationCode);
	}
}