using Lexiconner.Application.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Application.Middlewares
{
    /// <summary>
    /// Source: https://elanderson.net/2019/12/log-requests-and-responses-in-asp-net-core-3/
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory
        )
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>();
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            await LogRequest(context);
            await LogResponse(context);
        }
        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);

            _logger.LogInformation($"Http Request: " +
                                   $"{context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{(!string.IsNullOrEmpty(context.Request.QueryString.ToString()) ? context.Request.QueryString.ToString() : string.Empty)}; " +
                                   $"Request Body: {ReadStreamInChunksAsString(requestStream)}");

            context.Request.Body.Position = 0;
        }

        private async Task LogResponse(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            // execute request
            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // don't log response body for production
            if(HostingEnvironmentHelper.IsProductionAny() || true)
            {
                text = "(omitted for production)";
            }

            _logger.LogInformation($"Http Response: " +
                                   $"{context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{(!string.IsNullOrEmpty(context.Request.QueryString.ToString()) ? context.Request.QueryString.ToString() : string.Empty)}; " +
                                   $"Response Body: {text}");

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private static string ReadStreamInChunksAsString(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = reader.ReadBlock(readChunk,
                                                   0,
                                                   readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);
            return textWriter.ToString();
        }

    }
}
