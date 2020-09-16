using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
	public class Edit
	{
		public class Command : IRequest
		{
			public string DisplayName { get; set; }
			public string Bio { get; set; }
		}

		public class CommandValidator : AbstractValidator<Command>
		{
			public CommandValidator()
			{
				RuleFor(x => x.DisplayName).NotEmpty();
			}
		}

		public class Handler : IRequestHandler<Command>
		{
			private readonly DataContext context;
			private readonly IUserAccessor userAccessor;

			public Handler(DataContext context, IUserAccessor userAccessor)
			{
				this.context = context;
				this.userAccessor = userAccessor;
			}

			public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
			{
				var user = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == this.userAccessor.GetCurrentUsername());
				user.DisplayName = request.DisplayName;
				user.Bio = request.Bio ?? null;
				var success = await this.context.SaveChangesAsync() > 0;
				if (success) return Unit.Value;
				throw new Exception("Problem saving changes");
			}
		}
	}
}