using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Moar.Http
{

	public class HttpHandler : IHttpHandler
    {
		//const string DefaultPage = "default.html";

		AliasHandler	aliasHandler	= new AliasHandler();
		GroupHandler	userHandlers	= new GroupHandler();
		NotFoundHandler notFoundHandler = new NotFoundHandler();

		public static HttpHandler Default = new HttpHandler();

		public HttpHandler WithAlias(string aliasUrl, string redirectUrl)
		{
			aliasHandler.Add(aliasUrl, redirectUrl);
			return this;
		}

		public HttpHandler WithHandler(object handler)
		{
			userHandlers.Add(handler as IHttpHandler ?? new ObjectHandler(handler));
			return this;
		}

		public bool HandleRequest(HttpServerContext context)
        {
            try
            {
				return aliasHandler.HandleRequest(context)
					|| userHandlers.HandleRequest(context)
					|| notFoundHandler.HandleRequest(context);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} at HttpServer.HandleRequest: {1}", e, e.Message);
            }
			return false;
        }
	}
}
