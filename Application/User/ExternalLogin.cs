using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.User
{
	public class ExternalLogin
	{
		public class Query : IRequest<User>
		{
			public string AccessToken { get; set; }
		}
		public class Handler : IRequestHandler<Query, User>
		{
			private readonly UserManager<AppUser> userManager;
			private readonly IFacebookAccessor facebookAccessor;
			private readonly IJwtGenerator jwtGenerator;

			public Handler(UserManager<AppUser> userManager, IFacebookAccessor facebookAccessor, IJwtGenerator jwtGenerator)
			{
				this.userManager = userManager;
				this.facebookAccessor = facebookAccessor;
				this.jwtGenerator = jwtGenerator;
			}

			public async Task<User> Handle(Query request, CancellationToken cancellationToken)
			{
				var userInfo = await this.facebookAccessor.FacebookLogin(request.AccessToken);
				if (userInfo == null)
					throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem validationg token" });
				var user = await this.userManager.FindByEmailAsync(userInfo.Email);
				var refreshToken = this.jwtGenerator.GenerateRefreshToken();
				if (user != null)
				{
					user.RefreshTokens.Add(refreshToken);
					await this.userManager.UpdateAsync(user);
					return new User(user, this.jwtGenerator, refreshToken.Token);
				}
				user = new AppUser
				{
					DisplayName = userInfo.Name,
					Id = userInfo.Id,
					Email = userInfo.Email,
					UserName = "fb_" + userInfo.Id
				};
				var photo = new Photo
				{
					Id = "fb_" + userInfo.Id,
					Url = userInfo.Picture.Data.Url,
					IsMain = true
				};
				user.Photos.Add(photo);
				user.RefreshTokens.Add(refreshToken);
				var result = await this.userManager.CreateAsync(user);
				if (!result.Succeeded)
					throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem creating user" });
				return new User(user, this.jwtGenerator, refreshToken.Token);
			}
		}
	}
}