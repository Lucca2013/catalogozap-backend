using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CatalogoZap.Infrastructure.Exceptions;

namespace CatalogoZap.Infrastructure.Handlers;

public class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
        )
    {
        var (statusCode, title) = exception switch
        {
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Acess Unauthorized"),
            NotFoundException     => (StatusCodes.Status404NotFound, "Thing Not Found"),
            BadRequestException   => (StatusCodes.Status400BadRequest, "Invalid Request"),
            ForbiddenException    => (StatusCodes.Status403Forbidden, "Restricted Acess"),
            _                     => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; 
    }
}