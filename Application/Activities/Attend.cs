using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
	public class Attend
	{
		public class Command : IRequest
		{
			public Guid Id { get; set; }
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
				var activity = await this.context.Activities.FindAsync(request.Id);
				if (activity == null)
					throw new RestException(HttpStatusCode.NotFound, new { Activty = "Could not find activity" });
				var user = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == this.userAccessor.GetCurrentUsername());
				var attendance = await this.context.UserActivities.SingleOrDefaultAsync(x => x.ActivityId == activity.Id && x.AppUserId == user.Id);
				if (attendance != null)
					throw new RestException(HttpStatusCode.BadRequest, new { attendance = "Already attending this activity" });
				attendance = new UserActivity
				{
					Activity = activity,
					AppUser = user,
					IsHost = false,
					DateJoined = DateTime.Now
				};
				this.context.UserActivities.Add(attendance);
				var success = await this.context.SaveChangesAsync() > 0;
				if (success) return Unit.Value;
				throw new Exception("Problem saving changes");
			}
		}
	}
}