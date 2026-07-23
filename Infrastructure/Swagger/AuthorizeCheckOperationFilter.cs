using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CatalogoZap.Infrastructure.Swagger;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context) {
		// Check if the router has an [Authorize]
		var hasAuthorize =
			(
				context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
					.OfType<AuthorizeAttribute>()
					.Any() ?? false
			) || (
				context.MethodInfo?.GetCustomAttributes(true)
					.OfType<AuthorizeAttribute>()
					.Any() ?? false
			);

		if (!hasAuthorize) return;

		operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
		operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

		operation.Security = [
			new() {
				{
					new OpenApiSecurityScheme {
						Reference = new OpenApiReference {
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
					Array.Empty<string>()
				}
			}
		];
	}
}
