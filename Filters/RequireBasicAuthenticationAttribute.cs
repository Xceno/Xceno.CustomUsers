namespace Xceno.CustomUsers.Filters
{
	using System;
	using System.Net.Http;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Web.Http.Filters;
	using Results;
	using Orchard;
	using Orchard.Logging;
	using Services;

	/// <summary>
	/// Filter to enforce HTTP Basic authentication for the decorated WebApi controller/action.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class RequireBasicAuthenticationAttribute : Attribute, IAuthenticationFilter
	{
		public RequireBasicAuthenticationAttribute()
		{
			this.Logger = NullLogger.Instance;
		}

		public ILogger Logger { get; set; }

		public bool AllowMultiple
		{
			get
			{
				return false;
			}
		}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		{
			HttpRequestMessage request = context.Request;

			var workContext = context.ActionContext.ControllerContext.GetWorkContext();
			var authenticator = workContext.Resolve<IBasicAuthenticationService>();
			var credentials = authenticator.GetCredentials(request.Headers.Authorization);

			if ( credentials == null )
			{
				this.Logger.Warning(
					"Basic authentication failed: missing credentials {0} for {1}",
					request.Method,
					request.RequestUri);
				context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
				return;
			}

			var user = authenticator.GetUserForCredentials(credentials);
			if ( user == null )
			{
				this.Logger.Warning(
					"Basic authentication failed: invalid credentials {0} {1} for {2}",
					credentials.Username,
					request.Method,
					request.RequestUri);
				context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
			}
			else
			{
				authenticator.SetAuthenticatedUserForRequest(user);
			}
		}

		public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
		{
			// We do NOT return a Basic-Auth-Challenge header, so browser won't open an Auth-Dialog.
			////var challenge = new AuthenticationHeaderValue("Basic");
			////context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
			return Task.FromResult(0);
		}
	}
}