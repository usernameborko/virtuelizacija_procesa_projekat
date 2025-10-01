using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(IChargingService));
            host.Open();

            Console.WriteLine("Charging service is running...");
            Console.WriteLine("Press any key to stop the service.");
            Console.ReadKey();

            host.Close();
            Console.WriteLine("Service stopped.");
        }
    }
}
