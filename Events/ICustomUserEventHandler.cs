namespace Xceno.CustomUsers.Events
{
	using Models;
	using Orchard.Events;

	public interface ICustomUserEventHandler : IEventHandler
	{
		void CustomUserCreated(CustomUserPart customUserPart);

		void CustomUserActivated(CustomUserPart customUserPart);

		void CustomUserRequestedInvite(CustomUserPart customUserPart);

		void CustomUserRequestedInviteWasGranted(CustomUserPart customUserPart);
	}
}