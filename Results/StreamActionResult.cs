namespace Xceno.CustomUsers.Results
{
	using System.IO;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Web.Http;

	public class StreamActionResult : IHttpActionResult
	{
		private readonly string fileName;
		private readonly string mediaType;
		private readonly HttpRequestMessage request;

		public StreamActionResult(HttpRequestMessage request, Stream data, string fileName, string mediaType)
		{
			this.request = request;
			this.fileName = fileName;
			this.mediaType = mediaType;
			this.Content = new StreamContent(data);
			this.ContentLength = data.Length;
		}

		public long ContentLength { get; set; }

		public StreamContent Content { get; set; }

		public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
		{
			var response = this.request.CreateResponse(HttpStatusCode.OK);
			response.Content = this.Content;
			response.Content.Headers.ContentLength = this.ContentLength;
			response.Headers.CacheControl = new CacheControlHeaderValue();
			response.Content.Headers.ContentType = new MediaTypeHeaderValue(this.mediaType);
			response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
				{
					FileName = this.fileName,
					Size = this.ContentLength
				};

			return Task.FromResult(response);
		}
	}
}