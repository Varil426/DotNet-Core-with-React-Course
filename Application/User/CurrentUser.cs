using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence;

namespace Application.User
{
	public class CurrentUser
	{
		public class Query : IRequest<User> { }
		public class Handler : IRequestHandler<Query, User>
		{
			private readonly UserManager<AppUser> userManager;
			private readonly IJwtGenerator jwtGenerator;
			private readonly IUserAccessor userAccessor;

			public Handler(UserManager<AppUser> userManager, IJwtGenerator jwtGenerator, IUserAccessor userAccessor)
			{
				this.userManager = userManager;
				this.jwtGenerator = jwtGenerator;
				this.userAccessor = userAccessor;
			}

			public async Task<User> Handle(Query request, CancellationToken cancellationToken)
			{
				var user = await this.userManager.FindByNameAsync(this.userAccessor.GetCurrentUsername());
				var refreshToken = this.jwtGenerator.GenerateRefreshToken();
				user.RefreshTokens.Add(refreshToken);
				await this.userManager.UpdateAsync(user);
				return new User(user, this.jwtGenerator, refreshToken.Token);
			}
		}
	}
}