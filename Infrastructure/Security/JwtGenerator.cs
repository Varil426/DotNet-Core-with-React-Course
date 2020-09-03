using System.Collections.Generic;
using System.Security.Claims;
using Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Domain;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Security
{
	public class JwtGenerator : IJwtGenerator
	{
		private readonly SymmetricSecurityKey key;
		public JwtGenerator(IConfiguration config)
		{
			this.key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
		}

		public string CreateToken(AppUser user)
		{
			var claims = new List<Claim> {
				new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
			};
			// Generate signing credentials
			// Tokens are generated using "super secret key". It should never get out
			var creds = new SigningCredentials(this.key, SecurityAlgorithms.HmacSha512Signature);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddDays(7),
				SigningCredentials = creds
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}