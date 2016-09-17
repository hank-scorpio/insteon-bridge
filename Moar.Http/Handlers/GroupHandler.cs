using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Moar.Http
{
    public class GroupHandler : IHttpHandler
    {
    
    	List<IHttpHandler> handlers = new List<IHttpHandler>();
    
    	public GroupHandler(params IHttpHandler[] handlers)
    	{
    		this.handlers.AddRange(handlers);
    	}
    	public void Add(IHttpHandler handler)
    	{
    		handlers.Add(handler) ;
    	}
    
    	public bool HandleRequest(HttpServerContext context)
    	{
			return handlers.Any(h => h.HandleRequest(context));
    	}
    }
}
