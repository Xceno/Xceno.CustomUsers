namespace Xceno.CustomUsers.Activities
{
	using System.Collections.Generic;
	using Orchard.Localization;
	using Orchard.Workflows.Models;
	using Orchard.Workflows.Services;

	public abstract class MyCustomActivity : Event
	{
		protected MyCustomActivity()
		{
			this.T = NullLocalizer.Instance;
		}

		public Localizer T { get; set; }

		public override bool CanStartWorkflow
		{
			get { return true; }
		}

		public override LocalizedString Category
		{
			get { return this.T("Events"); }
		}

		public override bool CanExecute(WorkflowContext workflowContext, ActivityContext activityContext)
		{
			return true;
		}

		public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext)
		{
			return new[] { this.T("Done") };
		}

		public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext)
		{
			yield return this.T("Done");
		}
	}
}