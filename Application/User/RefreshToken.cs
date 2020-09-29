using System;
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
	public class RefreshToken
	{
		public class Command : IRequest<User>
		{
			public string RefreshToken { get; set; }
		}
		public class Handler : IRequestHandler<Command, User>
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

			public async Task<User> Handle(Command request, CancellationToken cancellationToken)
			{
				var user = await this.userManager.FindByNameAsync(this.userAccessor.GetCurrentUsername());
				var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == request.RefreshToken);
				if (oldToken != null && !oldToken.IsActive)
					throw new RestException(HttpStatusCode.Unauthorized);
				if (oldToken != null)
					oldToken.Revoked = DateTime.UtcNow;
				var newRefreshToken = this.jwtGenerator.GenerateRefreshToken();
				user.RefreshTokens.Add(newRefreshToken);
				await this.userManager.UpdateAsync(user);
				return new User(user, this.jwtGenerator, newRefreshToken.Token);
			}
		}
	}
}