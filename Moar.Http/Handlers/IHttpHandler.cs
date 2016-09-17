
namespace Moar.Http
{
    public interface IHttpHandler
    {
        bool HandleRequest(HttpServerContext context);
    }
	public static class IHttpHandlerExtensions
	{
		public static HttpServer Start(this IHttpHandler handler, int port = HttpServer.HttpPort)
		{
			var server = new HttpServer(handler, port);
			server.Start();
			return server;
		}
	}
}
