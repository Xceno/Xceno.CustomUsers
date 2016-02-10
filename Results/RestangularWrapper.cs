namespace Xceno.CustomUsers.Results
{
	public class RestangularWrapper<T> : RestangularMetaResult
	{
		public T Items { get; set; }
	}
}