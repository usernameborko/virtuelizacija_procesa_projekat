using System;
using System.ServiceModel;

namespace Service
{
    public class Program
    {
        static void Main(string[] args)
        {
            EventLogger logger = new EventLogger();

            ChargingService.OnTransferStarted += logger.OnTransferStarted;
            ChargingService.OnSampleReceived += logger.OnSampleReceived;
            ChargingService.OnTransferCompleted += logger.OnTransferCompleted;
            ChargingService.OnWarningRaised += logger.OnWarningRaised;

            ChargingService.OnVoltageSpike += logger.OnVoltageSpike;
            ChargingService.OnCurrentSpike += logger.OnCurrentSpike;

            ServiceHost host = new ServiceHost(typeof(ChargingService));
            host.Open();

            Console.WriteLine("Charging service is running...");
            Console.WriteLine("Event logging is active.");
            Console.WriteLine("Voltage and current spike analytics is active.");
            Console.WriteLine("Press any key to stop the service.");
            Console.ReadKey();

            host.Close();
            Console.WriteLine("Service stopped.");
        }
    }
}
