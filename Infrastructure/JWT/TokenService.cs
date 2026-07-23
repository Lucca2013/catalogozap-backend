using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CatalogoZap.Options.Token;
using Microsoft.Extensions.Options;

namespace CatalogoZap.Infrastructure.JWT;

public sealed class TokenService(IOptions<TokenOptions> options)
{
	public static TokenValidationParameters GetValidationParameters(string jwtKey, string jwtIssuer, string jwtAudience) => new()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

        ValidateIssuer = true,
        ValidateAudience = true,

        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

	public string GenerateToken(Guid userId, int expirationHours)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.JwtKey));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var claims = new[] {
			new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

		var token = new JwtSecurityToken(
			issuer: options.Value.JwtIssuer,
			audience: options.Value.JwtAudience,
			claims: claims,
			expires: DateTime.UtcNow.AddHours(expirationHours),
			signingCredentials: credentials
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public static Guid GetUserId(ClaimsPrincipal User)
	{
		var UserId = User.FindFirst(ClaimTypes.NameIdentifier);

		if (UserId == null)
			throw new UnauthorizedAccessException("UserId not found in the token.");

		return Guid.Parse(UserId.Value);
	}

	//for cases when UserId can be null
	public static Guid? TryGetUserId(ClaimsPrincipal user)
	{
		if (user?.Identity?.IsAuthenticated != true)
			return null;

		var claim = user.FindFirst(ClaimTypes.NameIdentifier);
		if (claim == null)
			return null;

		return Guid.Parse(claim.Value);
	}

}
