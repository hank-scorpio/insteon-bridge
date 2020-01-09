using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Homer.Insteon
{
	public class InsteonDevice
    {
        public InsteonId            Address	    { get; }
        public String			    Name	    { get; }
        public String			    Zone	    { get; }
        public String			    Alias	    { get; }
        public InsteonController    Controller  { get; }

        public InsteonDevice(InsteonId address, InsteonController controller, string name = null, string zone = null, string alias = null)
    	{
            Address = address;
            Controller = controller ?? this as InsteonController;
            Zone = zone ?? "";
            Name = name ?? address.ToString();
            Alias = alias ?? Name;
    	}

        public override string ToString() 
            => $"{{{GetType().Name} Addr={Address} Name={Name}}}";

    }
}
