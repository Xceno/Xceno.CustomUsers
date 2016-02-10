namespace Xceno.CustomUsers.Helpers
{
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using Orchard.ContentManagement;
	using Orchard.ContentManagement.Records;

	public class ContentItemComparer : IEqualityComparer<ContentItem>
	{
		public bool Equals(ContentItem x, ContentItem y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(ContentItem obj)
		{
			return obj.Id.GetHashCode();
		}
	}

	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single class", Justification = "Ok")]
	public class ContentItemRecordComparer : IEqualityComparer<ContentItemRecord>
	{
		public bool Equals(ContentItemRecord x, ContentItemRecord y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(ContentItemRecord obj)
		{
			return obj.Id.GetHashCode();
		}
	}

	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single class", Justification = "Ok")]
	public class ContentItemVersionRecordComparer : IEqualityComparer<ContentItemVersionRecord>
	{
		public bool Equals(ContentItemVersionRecord x, ContentItemVersionRecord y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(ContentItemVersionRecord obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}