using System;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Moar.Http
{

	//public class HttpNewContext
	//{
	//	public HttpNewContext(HttpServer server, HttpListenerContext listenerContext)
	//	{
	//		//Server = server;
	//	//	listenerContext.Request.

	//		if (Request.Cookies["SessionId"] == null)
	//		{
	//			Response.AppendCookie(new Cookie("SessionId", Guid.NewGuid().ToString()));
	//		}

	//		RequestPath = Request.Url.AbsolutePath.TrimStart(UrlDirectorySeparatorChar);
	//	}
	//}
    public class HttpServerContext
    {
        public HttpServer Server { get; protected set; }
        HttpListenerContext listenerContext;

        public const char UrlDirectorySeparatorChar = '/';

        public HttpServerContext(HttpServer server, HttpListenerContext listenerContext)
        {
            Server = server; 
            this.listenerContext = listenerContext;

            if (Request.Cookies["SessionId"] == null)
            {
                 Response.AppendCookie(new Cookie("SessionId", Guid.NewGuid().ToString()));
            }

            RequestPath =  Request.Url.AbsolutePath.TrimStart(UrlDirectorySeparatorChar);
        }
		public HttpServerContext Copy()
		{
			return new HttpServerContext(Server, listenerContext);
		}
        public string RequestPath { get; set; }

        public HttpListenerRequest Request { get { return listenerContext.Request; } }
        public HttpListenerResponse Response { get { return listenerContext.Response; } }
        public IPrincipal User { get { return listenerContext.User; } }

        public string SessionId { get { return Request.Cookies["SessionId"].Value; } }


        public void RespondXml(object response)
        {
            MemoryStream stream = new MemoryStream();
            new XmlSerializer(response.GetType()).Serialize(stream, response);
            stream.Position = 0;
            Respond(new StreamReader(stream).ReadToEnd(), MimeType.Xml);
        }

        public void RespondXml(XElement element)
        {
            Respond(element.ToString(), MimeType.Xml);
        }
        public void RespondHtml(string body, string title = "")
        {           
            Respond(String.Format("<html><title>{0}</title><body>{1}</body></html>", title, body),  MimeType.Html);
        }
        public void RespondHtmlMessage(string header, string message = "")
        {
            RespondHtml(String.Format("<h1>{0}</h1><p>{1}</p>", header, message), header);
        }
        public void RespondFile(string path, string mimeType = null)
        {
			if (File.Exists(path))
			{
				using (var fs = new FileStream(path, FileMode.Open))
				{
					RespondStream(fs, (mimeType ?? MimeType.GetFromPath(path)));
				}
			
			}
        }
        public void RespondUrl(string url)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(url);
            string mimeType = client.ResponseHeaders["Content-Type"];
            RespondStream(stream, mimeType);
        }

        public void Respond(object response, string mimeType)
        {
            if (response is string) Respond(response as string, mimeType);
            else if (response is byte[]) Respond(response as byte[], mimeType);
            else Respond(response, HttpFormat.Raw); 
        }
        public void Respond(string response, string mimeType = MimeType.Text)
        {
            Respond(Encoding.Default.GetBytes(response), mimeType);
        }
        public void Respond(byte[] response, string mimeType = MimeType.Binary)
        {
			//Response.StatusCode = 200;
			Response.ContentType = mimeType;
            Response.OutputStream.Write(response, 0, response.Length);
        }

        public void RespondJson(object response)
        {
            Respond(new JavaScriptSerializer().Serialize(response), MimeType.Json);
        }

        public void Respond(object response, HttpFormat format, string view = null)
        {


            switch (format)
            {
                case HttpFormat.Json:
                    RespondJson(response);
                    return;
                case HttpFormat.Xml:
                    RespondXml(response);
                    return;
                case HttpFormat.Raw:
					Type type = response.GetType();
					if (type == typeof(XElement)) RespondXml(response as XElement);
                    else if (type == typeof(string)) Respond(response as string);
                    else if (type == typeof(byte[])) Respond(response as byte[]);
                    else RespondJson(response);
                    return;
            }


        }
		public void RespondRedirect(string redirectPath)
		{
			var c = Copy();
			c.RequestPath = redirectPath;
			Server.Handler.HandleRequest(c);
		}
        public void RespondStream(Stream stream, string mimeType = MimeType.AutoDetect)
        {
            Response.ContentType = mimeType;
			StreamTools.CopyStream(stream, Response.OutputStream);
        }
        
    }
}
