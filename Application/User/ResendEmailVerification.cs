using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.User
{
	public class ResendEmailVerification
	{
		public class Query : IRequest
		{
			public string Email { get; set; }
			public string Origin { get; set; }
		}

		public class Handler : IRequestHandler<Query>
		{
			private readonly UserManager<AppUser> userManager;
			private readonly IEmailSender emailSender;

			public Handler(UserManager<AppUser> userManager, IEmailSender emailSender)
			{
				this.userManager = userManager;
				this.emailSender = emailSender;
			}
			public async Task<Unit> Handle(Query request, CancellationToken cancellationToken)
			{
				var user = await this.userManager.FindByEmailAsync(request.Email);
				var token = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
				token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
				var verifyUrl = $"{request.Origin}/user/verifyEmail?token={token}&email={request.Email}";
				var message = $"<p>Please click the below link to verify your email address:</p><p><a href='{verifyUrl}'>Click me!</a></p>";
				await this.emailSender.SendEmailAsync(request.Email, "Please verify email address", message);
				return Unit.Value;
			}
		}
	}
}