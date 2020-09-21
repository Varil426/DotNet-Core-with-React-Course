using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
	public class List
	{
		public class ActivitiesEnvelope
		{
			public List<ActivityDto> Activities { get; set; }
			public int ActivityCount { get; set; }
		}
		public class Query : IRequest<ActivitiesEnvelope>
		{
			public int? Limit { get; set; }
			public bool IsGoing { get; }
			public bool IsHost { get; }
			public DateTime? StartDate { get; }
			public int? Offset { get; set; }

			public Query(int? offset, int? limit, bool isGoing, bool isHost, DateTime? startDate)
			{
				Offset = offset;
				Limit = limit;
				IsGoing = isGoing;
				IsHost = isHost;
				StartDate = startDate ?? DateTime.Now;
			}
		}
		public class Handler : IRequestHandler<Query, ActivitiesEnvelope>
		{
			private readonly DataContext context;
			private readonly IMapper mapper;
			private readonly IUserAccessor userAccessor;

			public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
			{
				this.context = context;
				this.mapper = mapper;
				this.userAccessor = userAccessor;
			}

			public async Task<ActivitiesEnvelope> Handle(Query request, CancellationToken cancellationToken)
			{
				var queryable = this.context.Activities
					.Where(x => x.Date >= request.StartDate)
					.OrderBy(x => x.Date)
					.AsQueryable();
				if (request.IsGoing && !request.IsHost)
				{
					queryable = queryable.Where(x => x.UserActivities.Any(a => a.AppUser.UserName == this.userAccessor.GetCurrentUsername()));
				}
				else if (request.IsHost && !request.IsGoing)
				{
					queryable = queryable.Where(x => x.UserActivities.Any(a => a.AppUser.UserName == this.userAccessor.GetCurrentUsername() && a.IsHost));
				}
				var activities = await queryable.Skip(request.Offset ?? 0).Take(request.Limit ?? 3).ToListAsync();
				return new ActivitiesEnvelope
				{
					Activities = this.mapper.Map<List<Activity>, List<ActivityDto>>(activities),
					ActivityCount = queryable.Count()
				};
			}
		}
	}
}