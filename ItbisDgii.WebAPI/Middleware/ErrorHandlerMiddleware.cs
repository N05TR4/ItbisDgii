using ItbisDgii.Application.Exceptions;
using ItbisDgii.Application.Wrappers;
using System.Net;
using System.Text.Json;

namespace ItbisDgii.WebAPI.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

            }
            catch (Exception error)
            {
                var respose = context.Response;
                respose.ContentType = "application/json";
                var responseModel = new Response<string>() { Succeeded = false, Message = error?.Message };

                switch (error)
                {
                    case ApiExceptions e:
                        //custom application error
                        respose.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    case ValidationsExceptions e:
                        //custom application error
                        respose.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Errors = e.Errors;
                        break;

                    case KeyNotFoundException e:
                        //not found error
                        respose.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    default:
                        //unhandle error
                        respose.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(responseModel);

                await respose.WriteAsync(result);

            }
        }
    }
}
