using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Moar.Http
{

	public class ResourceFileHandler : IHttpHandler
    {
        public string BaseUrl { get; protected set; }
        public string AssemblyName { get; protected set; }

        const char ResourcePathSeparatorChar = '.';

        Assembly assembly;

        public ResourceFileHandler(string assemblyName, string baseUrl = "")
        {
            AssemblyName = assemblyName;
            BaseUrl = baseUrl;
            assembly = Assembly.Load(assemblyName);
        }

        #region IHttpHandler Members

        public bool HandleRequest(HttpServerContext context)
        {
            Stream stream = assembly.GetManifestResourceStream(AssemblyName + ResourcePathSeparatorChar + ToResourcePath(context.RequestPath));
            if (stream == null) return false;
            context.RespondStream(stream, MimeType.GetFromPath(context.RequestPath));            
            return true;            
        }

        public static string ToResourcePath(string requestPath)
        {
            return requestPath.Replace(HttpServerContext.UrlDirectorySeparatorChar, ResourcePathSeparatorChar);
        }

        #endregion




    }
}
