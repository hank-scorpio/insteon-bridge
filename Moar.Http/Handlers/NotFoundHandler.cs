using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Moar.Http
{

	public class NotFoundHandler : IHttpHandler
    {
		const string NotFoundResponse = "<html><head><title>404 Not Found</title></head><body><h1>404 Not Found</h1></body></html>";

		public bool HandleRequest(HttpServerContext context)
		{
			context.Response.StatusCode = 404;
			context.Respond(NotFoundResponse, MimeType.Html);
			return true;
		}
    }
}
