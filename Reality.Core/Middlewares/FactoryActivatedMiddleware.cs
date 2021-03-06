﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Reality.Core.Services;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Reality.Core.Middlewares
{
    public class FactoryActivatedMiddleware : IMiddleware
    {
        private readonly ITestService _db;

        public FactoryActivatedMiddleware(ITestService db)
        {
            _db = db;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var keyValue = context.Request.Query["username"];

            if (!string.IsNullOrWhiteSpace(keyValue))
            {

                await context.Response.WriteAsync(_db.GreetUser(keyValue));
            }

            await next(context);

            // var response = await FormatResponse(context.Response);

            //TODO: Save log to chosen datastore

            //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.

        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            var body = request.Body;

            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableRewind();

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            //...Then we copy the entire request stream into the new buffer.
            await request.Body.ReadAsync(buffer, 0, buffer.Length);

            //We convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
            request.Body = body;

            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return $"{response.StatusCode}: {text}";
        }
    }
}
