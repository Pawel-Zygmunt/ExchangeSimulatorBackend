using ExchangeSimulatorBackend.Exceptions;
using Newtonsoft.Json;

namespace ExchangeSimulatorBackend.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (BadHttpRequestException e)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(GenerateResponse(e));
            }
            catch (NotFoundHttpException e)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(GenerateResponse(e));
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync("Something went wrong");
            }
        }

        private object GenerateResponse(Exception e)
        {
            return e.Data.Count>0 ? new { errors = e.Data }: new { errors = new { nonFieldErrors = e.Message } };
        }
    }
}
