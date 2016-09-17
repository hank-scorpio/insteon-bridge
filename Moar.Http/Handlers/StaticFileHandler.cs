using System;
using System.IO;

namespace Moar.Http
{
    public class StaticFileHandler : IHttpHandler
    {
        public string BaseUrl { get; protected set; }
        public string LocalPath { get; protected set; }

        


        public StaticFileHandler(string localPath = ".", string baseUrl = "")
        {
            BaseUrl = baseUrl;
            LocalPath = Path.GetFullPath(localPath);
        }

        public bool HandleRequest(HttpServerContext context)
        {
			try
			{
				string path = Path.Combine(LocalPath, ToFilePath(context.RequestPath));
				if (!File.Exists(path))
					return false;
				context.RespondFile(path);
				return true;
			}
			catch (Exception e)
			{
				return false;
            }

   
        }

        public static string ToFilePath(string requestPath)
        {
            return requestPath.Replace(HttpServerContext.UrlDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
    }
}
