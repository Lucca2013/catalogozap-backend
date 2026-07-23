using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace CatalogoZap.Infrastructure.Swagger;

public class AutoTagOperationFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context) {
		if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor controllerAction) {
			var controllerName = controllerAction.ControllerName;
			var hasAuthorize =
				context.MethodInfo.GetCustomAttributes(true)
					.OfType<AuthorizeAttribute>()
					.Any()
				|| (context.MethodInfo.DeclaringType?
					.GetCustomAttributes(true)
					.OfType<AuthorizeAttribute>()
					.Any() ?? false);

			var tagName = hasAuthorize ? $"{controllerName} - Privado" : $"{controllerName} - Público";

			operation.Tags = [new() { Name = tagName }];
		}
	}
}
