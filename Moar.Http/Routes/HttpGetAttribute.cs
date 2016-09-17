using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Moar.Http
{
	//public class Constraints
	//{
	//	bool Verb(
	//}
	public class HttpGetAttribute : Attribute
    { 


        public string				View			{ get; set; }
		public HttpFormat			ResponseFormat	{ get; set; }
		public HttpGetAttribute		BaseAttribute	{ get; protected set; }

        public string				Name			{ get; protected set; }

        public MethodInfo			Method			{ get; protected set; }
        public PropertyInfo			Property		{ get; protected set; }

        public bool					IsMethod		{ get { return Method != null; } }
        public bool					IsProperty		{ get { return Property != null; } }


		UrlFormat urlFormat;

		public string Url
        {
            get { return urlFormat == null ? null: urlFormat.Format; }
            set { urlFormat = value == null ? null : new UrlFormat(value); }
        }

        public UrlMatch Matches(string url)
		{
			return urlFormat.Matches(url);
		}




        public HttpGetAttribute(string url = null, HttpFormat responseFormat = HttpFormat.Json)
        {
            ResponseFormat = responseFormat;
            Url = url;
        }

        public void Initialize(MethodInfo method, HttpGetAttribute baseAttribute)
        {
            Method = method;

            Initialize(method.Name, baseAttribute);
        }
        public void Initialize(PropertyInfo property, HttpGetAttribute baseAttribute)
        {
            Property = property;

            Initialize(property.Name, baseAttribute);
        }
        void Initialize(string name, HttpGetAttribute baseAttribute)
        {
            Name = name;
			BaseAttribute = baseAttribute;
			
			if (Url == null) Url = name;
			if (BaseAttribute != null)
				Url = BaseAttribute.Url + "/" + Url;

		}


        public bool HandleRequest(HttpServerContext context, object instance)
        {
            string url = context.Request.Url.AbsolutePath.Trim('/');
            UrlMatch match = Matches(url);
            if (!match.IsMatch) return false;

  
            if (IsMethod)
            {
                object[] args = GetMethodArgs(context, match);
                if (Method.ReturnType == typeof(void))
                {
                    Method.Invoke(instance, args);
                }
                else
                {
                    context.Respond(Method.Invoke(instance, args), ResponseFormat, View);
                }
            }
            else if (IsProperty)
            {
                context.Respond(Property.GetValue(instance, null), ResponseFormat, View);
            }
            return true;
        }

        object[] GetMethodArgs(HttpServerContext context, UrlMatch match)
        {
            ParameterInfo[] paramsInfo = Method.GetParameters();
            object[] args = new object[paramsInfo.Length];
            if (args.Length > 0 && match.MatchedArgs != null)
            {
                for (int i = 0; i < paramsInfo.Length; i++)
                {
                    if (i == 0 && paramsInfo[i].ParameterType == context.GetType())
                    {
                        args[i++] = context;
                    }
                    string value = "";
                    if (match.MatchedArgs.TryGetValue(paramsInfo[i].Name, out value) 
						|| match.MatchedArgs.TryGetValue(i.ToString(), out value))
                    {
						Type t = paramsInfo[i].ParameterType;
                        if (t == typeof(string))
						{
							args[i] = value;
                        }
						else if (t == typeof(int))
						{
							args[i] = int.Parse(value);
						}
						else
						{
							var m = t.GetMethod("Parse", new Type[] { typeof(string) });
							if (m != null)
							{
								try
								{
									args[i] = m.Invoke(null, new object[] { value });
								}
								catch
								{
									args[i] = null;
								}
							}
                        }            
                    }
                    else
                    {
                        args[i] = null;
                    }

                }
            }
            return args;
        }

      
    }


}
