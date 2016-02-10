namespace Xceno.CustomUsers.Results
{
	using System;
	using System.Collections.Generic;

	public class PagedResult<T>
	{
		public PagedResult(IEnumerable<T> items, int pageNo, int pageSize, long totalRecordCount)
		{
			this.Items = new List<T>(items);
			this.Meta = new Metadata(pageNo, pageSize, totalRecordCount);
		}

		public Metadata Meta { get; set; }

		public List<T> Items { get; set; }

		public class Metadata
		{
			public Metadata(int pageNo, int pageSize, long totalRecordCount)
			{
				this.PageNo = pageNo;
				this.PageSize = pageSize;
				this.TotalRecordCount = totalRecordCount;
				this.PageCount = this.TotalRecordCount > 0
					? (int) Math.Ceiling(this.TotalRecordCount/(double) this.PageSize)
					: 0;
			}

			public int PageNo { get; set; }

			public int PageSize { get; set; }

			public int PageCount { get; private set; }

			public long TotalRecordCount { get; set; }
		}
	}
}