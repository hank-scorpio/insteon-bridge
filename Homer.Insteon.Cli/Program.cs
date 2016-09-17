using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homer.Insteon.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var house = House.Load();

            house.UpdateAll().Wait();

        }
     



       
    }
}
