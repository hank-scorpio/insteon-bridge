using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Homer.Insteon.Cli
{

    class DemoProgram
    {
        static async void Run()
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
    }

}