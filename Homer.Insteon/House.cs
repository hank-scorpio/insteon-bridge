using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Homer.Insteon
{
	public class House
	{
		public string Name { get; }
		public IEnumerable<SwitchLinc> Lights { get; }
    	public IEnumerable<InsteonController> Controllers { get; }

        public House(XElement el)
        {
            Name = el.Attribute("Name").Value ?? nameof(House);

            Controllers =  el.Element("Controllers")
                .Elements()
                .Select(CreateController)
                .ToArray();

			Lights = el.Element("Devices")
                .Elements("Zone")
                .Elements()
                .Select(x => CreateDevice(x, Controllers.FirstOrDefault()))
                .OfType<SwitchLinc>()
                .ToArray();
        }


        public async Task UpdateAll()
            => await Task.WhenAll(Lights.Select(l => l.GetStatus()));

        #region XML Serialization


        public static House Load(string configFile = "insteon.xml")
		    => new House(XDocument.Load(configFile).Root.Element(nameof(House)));
		
        public static InsteonController CreateController(XElement el)
		{
            switch (el.Name.ToString())
			{
                case nameof(Hub):
                    return new Hub(
                        el.Attribute("Host").Value, 
                        el.Attribute("Username").Value,
                        el.Attribute("Password").Value);
				case nameof(SmartLinc):
                    return new SmartLinc(
                        el.Attribute("Host").Value);
			}
            return null;
		}

        public static InsteonDevice CreateDevice(XElement el, InsteonController c)
		{
            switch (el.Name.ToString())
			{
                case nameof(SwitchLinc):
				case "Light":
                    return new SwitchLinc(
                        c,
                        el.Attribute("Id")?.Value,
                        el.Attribute("Name")?.Value,
                        el.Ancestors("Zone")?.FirstOrDefault()?.Attribute("Name")?.Value,
                        el.Attribute("Alias")?.Value,
                        LightLevelCurve.GetByNameOrDefault(el.Attribute("Type")?.Value));
            }
            return null;
		}
      
        #endregion
    }
}
