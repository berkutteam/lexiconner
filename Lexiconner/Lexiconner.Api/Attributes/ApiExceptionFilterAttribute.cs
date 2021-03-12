using Lexiconner.Application.Exceptions;
using Lexiconner.Application.Extensions;
using Lexiconner.Domain.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Lexiconner.Api.Attributes
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ApiExceptionFilterAttribute> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public override void OnException(ExceptionContext context)
        {
            // check if request to API
            bool isRequestToApi = context.HttpContext.Request.Path.HasValue && context.HttpContext.Request.Path.Value.StartsWith("/api");
            if (isRequestToApi)
            {
                _logger.LogError($"Exception handler caught error: {context.Exception}");

                // set generic user friendly error
                string title = "Error happened.";
                string message = "Looks like we are experiencing some problems. Please, try again.";

                // display messages from exceptions that inherit ApplicationBaseException as they should contain user friendly messages
                // other exceptions' messages aren't suitable to display
                if (context.Exception is ApplicationBaseException)
                {
                    title = (context.Exception as ApplicationBaseException).Title;
                    message = context.Exception.Message;
                }

                var errors = new Dictionary<string, string[]>()
                {
                    { "", new string[] { message } },
                };

                if (_hostingEnvironment.IsDevelopmentAny() || _hostingEnvironment.IsTestingAny())
                {
                    // return full exception details with StackTrace
                    errors.Add("DebugStackTrace", new string[] { $"{context.Exception.Message}.\nStackTrace: {context.Exception.StackTrace}" });
                }

                switch (context.Exception)
                {
                    case AccessDeniedException specificException:
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        break;

                    case UnauthorizedException specificException:
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;

                    case NotFoundException specificException:
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    case BadRequestException specificException:
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    case ValidationErrorException specificException:
                        // use BadRequest (400) instead of UnprocessableEntity (422) for custom validation response as MVC uses BadRequest
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                        // reset 
                        errors.Clear();

                        // add validation error details
                        foreach (var failure in specificException.ValidationFailures)
                        {
                            if (string.IsNullOrEmpty(failure.PropertyName))
                            {
                                if (!errors.ContainsKey(string.Empty))
                                {
                                    errors.Add(string.Empty, new string[] { });
                                }
                                errors[string.Empty] = errors[string.Empty].ToList().Concat(new string[] { failure.ErrorMessage }).ToArray();
                            }
                            else
                            {
                                errors.Add(failure.PropertyName, new string[] { failure.ErrorMessage });
                            }
                        }
                        break;

                    case InternalErrorException specificException:
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;

                    default:
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                };

                var result = new ValidationProblemDetails(errors)
                {
                    Instance = context.HttpContext.Request.Path,
                    Type = string.Empty,
                    Title = title,
                    Status = context.HttpContext.Response.StatusCode,
                    Detail = string.Empty, // human-readable explanation
                };

                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(result, SerializationConfig.GetDefaultJsonSerializerSettings())).GetAwaiter().GetResult();
                context.ExceptionHandled = true;
            }

            base.OnException(context);
        }
    }
}
