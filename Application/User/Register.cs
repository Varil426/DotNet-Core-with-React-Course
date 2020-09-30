using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Application.Validators;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.User
{
	public class Register
	{
		public class Command : IRequest
		{
			public string DisplayName { get; set; }
			public string Username { get; set; }
			public string Email { get; set; }
			public string Password { get; set; }
			public string Origin { get; set; }

		}

		public class CommandValiadtor : AbstractValidator<Command>
		{
			public CommandValiadtor()
			{
				RuleFor(x => x.DisplayName).NotEmpty();
				RuleFor(x => x.Username).NotEmpty();
				RuleFor(x => x.Email).NotEmpty().EmailAddress();
				RuleFor(x => x.Password).Password();
			}
		}

		public class Handler : IRequestHandler<Command>
		{
			private readonly DataContext context;
			private readonly UserManager<AppUser> userManager;
			private readonly IEmailSender emailSender;

			public Handler(DataContext context, UserManager<AppUser> userManager, IEmailSender emailSender)
			{
				this.context = context;
				this.userManager = userManager;
				this.emailSender = emailSender;
			}

			public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
			{
				if (await this.context.Users.AnyAsync(x => x.Email == request.Email))
					throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists" });
				if (await this.context.Users.AnyAsync(x => x.UserName == request.Username))
					throw new RestException(HttpStatusCode.BadRequest, new { Username = "Username already exists" });
				var user = new AppUser
				{
					DisplayName = request.DisplayName,
					Email = request.Email,
					UserName = request.Username
				};
				var result = await this.userManager.CreateAsync(user, request.Password);
				if (!result.Succeeded)
					throw new Exception("Problem saving changes");
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