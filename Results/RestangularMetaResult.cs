#pragma warning disable SA1402 // File may only contain a single class
namespace Xceno.CustomUsers.Results
{
	public class RestangularMetaResult
	{
		public dynamic Meta { get; set; }
	}

	public class RestangularMetaResult<T>
	{
		public T Meta { get; set; }
	}
}
#pragma warning restore SA1402 // File may only contain a single class