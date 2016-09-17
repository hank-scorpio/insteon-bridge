using System.Linq;

namespace Moar.Http
{
	public class ObjectHandler : IHttpHandler
	{
		HttpGetAttribute[] getAttributes;

		public object Instance { get; protected set; }

		public ObjectHandler(object instance)
		{
			Instance = instance;
			var baseAttribute = Instance.GetType()
				.GetCustomAttributes(typeof(HttpGetAttribute), true).Cast<HttpGetAttribute>().FirstOrDefault();

            getAttributes = Enumerable.Concat
			(
				Instance.GetType().GetMethods()
					.SelectMany(m => m.GetCustomAttributes(typeof(HttpGetAttribute), true).Cast<HttpGetAttribute>()
					.Each(at => at.Initialize(m, baseAttribute))),

				Instance.GetType().GetProperties().Where(x => x.CanRead).SelectMany(p => p.GetCustomAttributes(typeof(HttpGetAttribute), true)
					.Cast<HttpGetAttribute>()
					.Each(at => at.Initialize(p, baseAttribute)))
			).ToArray();
		}
		public bool HandleRequest(HttpServerContext context)
		{
			return getAttributes.Any(at => at.HandleRequest(context, Instance));
		}


	  
	}


}
