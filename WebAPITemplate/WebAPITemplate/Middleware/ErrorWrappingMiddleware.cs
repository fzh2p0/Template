using System.Net;
using System.Text.Json;

namespace WebAPITemplate.Middleware;

/// <summary>
///     Middleware for error handling. Is registered in the middleware pipeline prior to UseMvc
/// </summary>
internal sealed class ErrorWrappingMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    ///     Set the next middleware piece
    /// </summary>
    /// <param name="next">Next piece of middleware in the pipeline</param>
    public ErrorWrappingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    ///     Invoke method that is called from the pipeline. This method invokes the next middleware piece, then
    ///     returns a fitting response based on the response of the next piece.
    /// </summary>
    /// <param name="context">The Http context</param>
    /// <returns> a fitting response based on the response of the next piece.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);

            // Get the standard text for the returned status
            var message = ((HttpStatusCode)context.Response.StatusCode).ToString();

            // If something went wrong, (other than NotFound and BadRequest which are handled in the controller) handle the errors here.
            // Otherwise drop through
            if (context.Response.StatusCode != (int)HttpStatusCode.OK &&
                context.Response.StatusCode != (int)HttpStatusCode.NotFound &&
                context.Response.StatusCode != (int)HttpStatusCode.BadRequest)
            {
                var errorResponse = new
                {
                    context.Response.StatusCode,
                    Message = $"An error {message} occurred while processing the request.",
                    context.Request.Method,
                    context.Request.Path
                };

                // Serialize the error response and return it to the client.
                var json = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(json);
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions and transform them into an error response.
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Create an error response object.
        var errorResponse = new
        {
            context.Response.StatusCode,
            Message = "An error occurred while processing the request.",
            ExceptionMessage = exception.Message
        };

        // Serialize the error response and return it to the client.
        var json = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(json);
    }
}