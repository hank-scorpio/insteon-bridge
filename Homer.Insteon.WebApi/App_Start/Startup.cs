using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Homer.Insteon.WebApi.Startup))]
namespace Homer.Insteon.WebApi
{
    
    public partial class Startup
    {
        
        public void Configuration(IAppBuilder app)
        {
            Insteon.House.UpdateAll().Wait();
        }
    }

    public static class Insteon
    {
        static string AppDataPath => AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
        static string HouseXmlFile => Path.Combine(AppDataPath, "maison.xml");

        public static House House { get; }
            = House.Load(HouseXmlFile);
    }
}
