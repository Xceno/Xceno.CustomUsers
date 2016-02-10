namespace Xceno.CustomUsers.Helpers
{
	using System.Collections.Generic;
	using Models;

	public class CustomUserRecordComparer : IEqualityComparer<CustomUserRecord>
	{
		public bool Equals(CustomUserRecord x, CustomUserRecord y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(CustomUserRecord obj)
		{
			return obj.Id.GetHashCode();
		}
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single class", Justification = "Ok")]
	public class CustomUserPartComparer : IEqualityComparer<CustomUserPart>
	{
		public bool Equals(CustomUserPart x, CustomUserPart y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(CustomUserPart obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}