using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Moar.Http
{
	public class AliasHandler : IHttpHandler
    {
    	Dictionary<string, string> aliases = new Dictionary<string, string>();
    
    	public bool HandleRequest(HttpServerContext context)
    	{
    		string alias = null;
			if (aliases.TryGetValue(context.RequestPath, out alias))
    		{
    			context.RespondRedirect(alias);
				return true;
    		}
			return false;
		}

		public void Add(string aliasUrl, string redirectUrl)
		{
			aliases.Add(aliasUrl, redirectUrl);
		}

	}
}
