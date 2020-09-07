using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
	public class Details
	{
		public class Querry : IRequest<ActivityDto>
		{
			public Guid Id { get; set; }
		}

		public class Handler : IRequestHandler<Querry, ActivityDto>
		{
			private readonly DataContext context;
			private readonly IMapper mapper;

			public Handler(DataContext context, IMapper mapper)
			{
				this.context = context;
				this.mapper = mapper;
			}

			public async Task<ActivityDto> Handle(Querry request, CancellationToken cancellationToken)
			{
				var activity = await this.context.Activities
					.FindAsync(request.Id);
				if (activity == null)
					throw new RestException(HttpStatusCode.NotFound, new { activity = "Not found" });
				var activityToReturn = this.mapper.Map<Activity, ActivityDto>(activity);
				return activityToReturn;
			}
		}
	}
}