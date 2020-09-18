using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
	public class ProfileReader : IProfileReader
	{
		private readonly DataContext context;
		private readonly IUserAccessor userAccessor;

		public ProfileReader(DataContext context, IUserAccessor userAccessor)
		{
			this.context = context;
			this.userAccessor = userAccessor;
		}

		public async Task<Profile> ReadProfile(string username)
		{
			var user = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == username);
			if (user == null)
				throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });
			var currentUser = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == this.userAccessor.GetCurrentUsername());
			var profile = new Profile
			{
				DisplayName = user.DisplayName,
				Username = user.UserName,
				Image = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
				Photos = user.Photos,
				Bio = user.Bio,
				FollowersCount = user.Followers.Count(),
				FollowingCount = user.Followings.Count()
			};
			if (currentUser.Followings.Any(x => x.TargetId == user.Id))
				profile.IsFollowed = true;
			return profile;
		}
	}
}