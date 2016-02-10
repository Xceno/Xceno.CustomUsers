namespace Xceno.CustomUsers.Results
{
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Web.Http;

	public class AddChallengeOnUnauthorizedResult : IHttpActionResult
	{
		public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IHttpActionResult innerResult)
		{
			this.Challenge = challenge;
			this.InnerHttpResult = innerResult;
		}

		public AuthenticationHeaderValue Challenge { get; private set; }

		public IHttpActionResult InnerHttpResult { get; private set; }

		public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
		{
			HttpResponseMessage response = await this.InnerHttpResult.ExecuteAsync(cancellationToken);

			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				// Only add one challenge per authentication scheme.
				if (!response.Headers.WwwAuthenticate.Any((h) => h.Scheme == this.Challenge.Scheme))
				{
					response.Headers.WwwAuthenticate.Add(this.Challenge);
				}
			}

			return response;
		}
	}
}