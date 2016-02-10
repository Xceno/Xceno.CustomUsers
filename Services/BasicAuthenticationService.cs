namespace Xceno.CustomUsers.Services
{
	using System;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using Models;
	using Orchard;
	using Orchard.Security;

	public class BasicAuthenticationService : IBasicAuthenticationService
	{
		private readonly IAuthenticationService authService;

		private readonly IMembershipService memberService;

		public BasicAuthenticationService(IMembershipService memberService, IAuthenticationService authService)
		{
			this.memberService = memberService;
			this.authService = authService;
		}

		public BasicAuthenticationCredentials GetCredentials(AuthenticationHeaderValue header)
		{
			if ( header == null || header.Scheme != "Basic" || string.IsNullOrEmpty(header.Parameter) )
			{
				return null;
			}

			var credentials = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(header.Parameter));
			int separatorIndex = credentials.IndexOf(':');

			if ( separatorIndex < 0 )
			{
				return null;
			}

			return new BasicAuthenticationCredentials
					   {
						   Username = credentials.Substring(0, separatorIndex),
						   Password = credentials.Substring(separatorIndex + 1)
					   };
		}

		public IUser GetUserForCredentials(BasicAuthenticationCredentials credentials)
		{
			return this.GetUserForCredentials(credentials, this.memberService);
		}

		public bool SetAuthenticatedUserForRequest(IUser user)
		{
			return this.SetAuthenticatedUserForRequest(user, this.authService);
		}

		public bool SetAuthenticatedUserForRequest(HttpRequestMessage request, WorkContext workContext)
		{
			var membershipService = workContext.Resolve<IMembershipService>();
			var authenticationService = workContext.Resolve<IAuthenticationService>();

			var credentials = this.GetCredentials(request.Headers.Authorization);
			var user = this.GetUserForCredentials(credentials, membershipService);
			return this.SetAuthenticatedUserForRequest(user, authenticationService);
		}

		private IUser GetUserForCredentials(BasicAuthenticationCredentials credentials, IMembershipService membershipService)
		{
			if ( credentials == null )
			{
				return null;
			}

			return membershipService.ValidateUser(credentials.Username, credentials.Password);
		}

		private bool SetAuthenticatedUserForRequest(IUser user, IAuthenticationService authenticationService)
		{
			if ( user == null )
			{
				return false;
			}

			authenticationService.SetAuthenticatedUserForRequest(user);

			return true;
		}
	}
}