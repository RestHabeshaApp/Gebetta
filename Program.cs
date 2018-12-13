using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System.Diagnostics;
using NServiceBus.Logging;

namespace StandardMVC
{
    public class Program
    {
        static ILog log = LogManager.GetLogger<Program>();

        public  static void Main(string[] args)
        {

//          CreateWebHostBuilder(args).Build().Run();

            AsyncMain().GetAwaiter().GetResult();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();


        static async Task AsyncMain()
        {
           
            var endpointConfiguration = new EndpointConfiguration("Messaging");

            var transport = endpointConfiguration.UseTransport<LearningTransport>();

            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Debug.Write("Press Enter to exit - Check at command...    ");

            Console.Write("Messaging");

        

            await RunLoop(endpointInstance).ConfigureAwait(false);

            await endpointInstance.Stop().ConfigureAwait(false);
        }


        static async Task RunLoop(IEndpointInstance endpointInstance)
        {
            while (true)
            {
                log.Info("Press 'P' to place an order, or 'Q' to quit.");
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.P:
                        // Instantiate the command
                        var command = new NServiceBus.PlaceOrder
                        {
                            OrderId = "Messaging Test " + Guid.NewGuid().ToString()
                        };

                        // Send the command to the local endpoint
                        log.Info($"Sending PlaceOrder command, OrderId = {command.OrderId}");
                        await endpointInstance.SendLocal(command)
                            .ConfigureAwait(false);

                        break;

                    case ConsoleKey.Q:
                        return;

                    default:
                        log.Info("Unknown input. Please try again.");
                        break;
                }
            }
        }

    }
}
