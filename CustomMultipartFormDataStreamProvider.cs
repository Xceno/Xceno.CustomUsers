namespace Xceno.CustomUsers
{
	using System.Net.Http;
	using System.Net.Http.Headers;

	public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
	{
		private readonly string fileNameOverride;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomMultipartFormDataStreamProvider" /> class.
		/// Creates a new CustomMultipartFormDataStreamProvider
		/// </summary>
		/// <param name="path">
		/// The path to save the files to
		/// </param>
		/// <param name="fileNameOverride">
		/// The files will be saved with this filename (the extension will be appended). Leave empty to use the original filename
		/// </param>
		public CustomMultipartFormDataStreamProvider(string path, string fileNameOverride = null)
			: base(path)
		{
			this.fileNameOverride = fileNameOverride;
		}

		public override string GetLocalFileName(HttpContentHeaders headers)
		{
			var hasFilename = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName);
			var name = "NoName";

			if ( hasFilename && !string.IsNullOrWhiteSpace(this.fileNameOverride) )
			{
				var filename = headers.ContentDisposition.FileName;
				var extension = filename.Substring(filename.LastIndexOf('.'));

				name = string.Format("{0}{1}", this.fileNameOverride, extension);
			}
			else if ( hasFilename )
			{
				////name = Statics.SanitizeFileName(headers.ContentDisposition.FileName);
			}

			return name.Replace("\"", string.Empty); // this is here because Chrome submits files in quotation marks which get treated as part of the filename and get escaped
		}
	}
}