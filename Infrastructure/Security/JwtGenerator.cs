using System.Collections.Generic;
using System.Security.Claims;
using Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Domain;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

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
				Expires = DateTime.Now.AddMinutes(15),
				SigningCredentials = creds
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		public RefreshToken GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);
			return new RefreshToken
			{
				Token = Convert.ToBase64String(randomNumber)
			};
		}
	}
}