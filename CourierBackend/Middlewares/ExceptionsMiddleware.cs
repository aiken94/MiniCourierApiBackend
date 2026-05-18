using System.Net;
using System.Text.Json;
using CourierBackend.Exceptions;
using CourierBackend.Responses;

namespace CourierBackend.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var response = new ApiErrorResponse();

        int statusCode;

        response.Status = "error";

        switch (exception)
        {
            case BadRequestException:
                statusCode = (int)HttpStatusCode.BadRequest;
                response.Message = exception.Message;
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An unexpected error occurred";
                break;
        }

        context.Response.ContentType = "application/json";

        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}