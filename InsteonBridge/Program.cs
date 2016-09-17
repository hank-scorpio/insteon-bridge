using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moar.Http;
using System.Dynamic;
using System.Linq.Expressions;
using Homer.Insteon;
using Infodev.CommLib.Ports.Log;
using Infodev.CommLib.Ports;
using System.Threading;

namespace InsteonBridge
{
	public class Program
	{
        enum Kitchen
        {
            Island   = 0x1DBBE6,
            Counter  = 0x1DD4A7,
        }
        static void Main(string[] args)
        {
            //MainAsync().Wait();
            // StartHttp();
            Main2();
        }

       
        static async void Main3()
        {
    
            
            
            using (var c = new SmartLinc("insteon.home"))
            {
                
                // Insteon IDs can be created from uint/long/ulong or string
                InsteonId kitchenSpotsId    = 0x1DD4A7;
                InsteonId kitchenIslandId   = "1D-BB-E6";

                
                // Get status using InsteonController.GetStatusAsync
                LightStatus status = await c.GetStatus(kitchenSpotsId);

                Console.WriteLine($"Success: {status.Result}, Level: {status.Level:X2}");

                // Create LightSwich object
                SwitchLinc light = new SwitchLinc(c, kitchenIslandId);

                // Status is updated automatically when expired/invalid
                Console.WriteLine($"Status: {light.Status}");


                // Status is also updated as a result of set operations
                Console.WriteLine($"Status: {await light.SetLevel(0.5)}");
         


                await c.Run();
            }

            Console.Read();

        }


		static async void StartHttp()
		{
            var house = House.Load();

            await house.UpdateAll();
     
            await HttpHandler.Default
                .WithHandler(new LightsApi(house))
                .WithHandler(new StaticFileHandler(@"..\..\Web"))
                .WithAlias("", "index.html")
                .Start(800)
                .Wait();
        }

        static void Main2()
        {
            var house = House.Load();

            house.UpdateAll().Wait();


            // Create SmartLinc controller from endpoint tcp://insteon.lan:9761

           // Hub hub = new Hub("insteon.home", "Cartwrig", "SkUOemxQ");

            // Create SwitchLinc controller from Insteon address 1D.D4.A7
          //  SwitchLinc switchLinc = hub.CreateSwitch(0x1DD4A7);

            // Status is updated automatically when invalid/expired
        //    var level = switchLinc.Status.LevelPct;

            //// Set off and check result
            //if (!switchLinc.SetOff().Succeeded) return;

            //// Set level to 50% (async)
           // switchLinc.SetLevel(0.15);
           // level = switchLinc.Status.LevelPct;
            //// Ramp level up for 2 seconds (async)
            //await switchLinc.RampUpAsync(2000);
            //// Dim
            //level = (await switchLinc.DimAsync()).Level.Value;
        }

	}
}
