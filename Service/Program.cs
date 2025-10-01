using System;
using System.ServiceModel;

namespace Service
{
    public class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(ChargingService));
            host.Open();

            Console.WriteLine("Charging service is running...");
            Console.WriteLine("Press any key to stop the service.");
            Console.ReadKey();

            host.Close();
            Console.WriteLine("Service stopped.");
        }
    }
}
