using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CatalogoZap.Infrastructure.JWT;

public interface ITokenService
{
	string GenerateToken(Guid userId, int expirationHours);
}

public class TokenService : ITokenService
{
	private readonly IConfiguration _config;

	public TokenService(IConfiguration config)
	{
		_config = config;
	}

	public static TokenValidationParameters GetValidationParameters(IConfiguration config) => new()
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(config["JWT_KEY"]!)
		),

		ValidateIssuer = true,
		ValidateAudience = true,

		ValidIssuer = config["JWT_ISSUER"],
		ValidAudience = config["JWT_AUDIENCE"],

		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero
	};

	public string GenerateToken(Guid userId, int expirationHours)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT_KEY"]!));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var claims = new[] {
			new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

		var token = new JwtSecurityToken(
			issuer: _config["JWT_ISSUER"],
			audience: _config["JWT_AUDIENCE"],
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
