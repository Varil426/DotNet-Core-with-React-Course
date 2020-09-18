using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
	public class Details
	{
		public class Query : IRequest<Profile>
		{
			public string Username { get; set; }
		}

		public class Handler : IRequestHandler<Query, Profile>
		{
			private readonly IProfileReader profileReader;

			public Handler(IProfileReader profileReader)
			{
				this.profileReader = profileReader;
			}

			public async Task<Profile> Handle(Query request, CancellationToken cancellationToken)
			{
				return await this.profileReader.ReadProfile(request.Username);
			}
		}
	}
}