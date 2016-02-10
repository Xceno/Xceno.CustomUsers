namespace Xceno.CustomUsers.Handlers
{
	using System.Collections.Generic;
	using Models;
	using Orchard.Workflows.Services;
	using Events;

	public class WorkflowCustomUserEventHandler : ICustomUserEventHandler
	{
		private readonly IWorkflowManager workflowManager;

		public WorkflowCustomUserEventHandler(IWorkflowManager workflowManager)
		{
			this.workflowManager = workflowManager;
		}

		public void CustomUserCreated(CustomUserPart customUserPart)
		{
			this.workflowManager.TriggerEvent(
				"CustomUserCreated",
				customUserPart.ContentItem,
				() => new Dictionary<string, object>
					  {
						  { "CustomUser", customUserPart.ContentItem },
						  { "User", customUserPart.User }
					  });
		}

		public void CustomUserActivated(CustomUserPart customUserPart)
		{
			this.workflowManager.TriggerEvent(
				"CustomUserActivated",
				customUserPart.ContentItem,
				() => new Dictionary<string, object>
					  {
						  { "CustomUser", customUserPart.ContentItem },
						  { "User", customUserPart.User }
					  });
		}

		public void CustomUserRequestedInvite(CustomUserPart customUserPart)
		{
			this.workflowManager.TriggerEvent(
				"CustomUserRequestedInvite",
				customUserPart.ContentItem,
				() => new Dictionary<string, object>
					  {
						  { "CustomUser", customUserPart.ContentItem },
						  { "User", customUserPart.User }
					  });
		}

		public void CustomUserRequestedInviteWasGranted(CustomUserPart customUserPart)
		{
			this.workflowManager.TriggerEvent(
				"CustomUserRequestedInviteWasGranted",
				customUserPart.ContentItem,
				() => new Dictionary<string, object>
					  {
						  { "CustomUser", customUserPart.ContentItem },
						  { "User", customUserPart.User }
					  });
		}
	}
}