namespace Xceno.CustomUsers.Handlers
{
	using System;
	using System.Linq;
	using Models;
	using Orchard.ContentManagement;
	using Orchard.ContentManagement.Handlers;
	using Orchard.Core.Common.Models;
	using Orchard.Data;
	using Orchard.Users.Models;

	public class CustomUserPartHandler : ContentHandler
	{
		private const string ContentType = "CustomUser";
		private readonly IContentManager contentManager;

		public CustomUserPartHandler(IRepository<CustomUserRecord> repository, IContentManager contentManager)
		{
			this.contentManager = contentManager;
			this.Filters.Add(StorageFilter.For(repository));
			this.Filters.Add(new ActivatingFilter<CustomUserPart>(ContentType));
			this.Filters.Add(new ActivatingFilter<CommonPart>(ContentType));

			this.OnActivated<CustomUserPart>(this.SetupCustomUserPart);
		}

		protected override void GetItemMetadata(GetContentItemMetadataContext context)
		{
			var part = context.ContentItem.As<CustomUserPart>();

			if ( part != null && part.User != null )
			{
				context.Metadata.Identity.Add("CustomUser.UserName", part.User.UserName);
				context.Metadata.DisplayText = part.User.UserName;
			}
		}

		private void SetupCustomUserPart(ActivatedContentContext context, CustomUserPart customUserPart)
		{
			customUserPart.UserLazyField.Loader(() =>
				customUserPart.Record.UserPartRecord == null ? null : this.contentManager.Get<UserPart>(customUserPart.Record.UserPartRecord.Id));

			customUserPart.UserLazyField.Setter(userPart =>
			{
				if ( userPart != null )
				{
					var exists =
						this.contentManager.Query<CustomUserPart, CustomUserRecord>()
							.Where(x => x.UserPartRecord != null && x.UserPartRecord.Id == userPart.Record.Id)
							.List()
							.Any();

					if ( exists )
					{
						throw new ArgumentException("This user record is already assigned to a custom user");
					}

					customUserPart.Record.UserPartRecord = userPart.Record;
				}
				else
				{
					customUserPart.Record.UserPartRecord = null;
				}

				return userPart;
			});
		}
	}
}