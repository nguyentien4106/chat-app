using EzyChat.Application.Exceptions;
using EzyChat.Domain.Exceptions;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace EzyChat.Api.Middlewares;

public class GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(
            "Error Message: {exceptionMessage}, Time of occurrence {time}",
            exception.Message, DateTime.Now);

        var details = exception switch
        {
            NotFoundException notFoundEx => new
            {
                IsSuccess = false,
                Errors = new[] { notFoundEx.Message },
                Data = (object?)null
            },
            UnauthorizedException unauthorizedEx => new
            {
                IsSuccess = false,
                Errors = new[] { unauthorizedEx.Message },
                Data = (object?)null
            },
            UnauthorizedAccessException => new
            {
                IsSuccess = false,
                Errors = new[] { "Unauthorized access" },
                Data = (object?)null
            },
            BadRequestException badRequestEx => new
            {
                IsSuccess = false,
                Errors = new[] { badRequestEx.Message },
                Data = (object?)null
            },
            ArgumentException argEx => new
            {
                IsSuccess = false,
                Errors = new[] { argEx.Message },
                Data = (object?)null
            },
            _ => new
            {
                IsSuccess = false,
                Errors = new[] { "An internal server error occurred" },
                Data = (object?)null
            }
        };

        var statusCode = exception switch
        {
            NotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedException => (int)HttpStatusCode.Unauthorized,
            BadRequestException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Title = exception.GetType().Name,
            Detail = string.Join(", ", details.Errors),
            Status = statusCode,
            Instance = context.Request.Path
        };
        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);
        
        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

        return true;
    }
} 