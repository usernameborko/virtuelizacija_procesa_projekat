using Common;
using System;
using System.ServiceModel;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IChargingService> factory = new ChannelFactory<IChargingService>("ChargingService");
            IChargingService proxy = factory.CreateChannel();

            try
            {
                string vehicleId = "golf_4_001";

                Console.WriteLine("Starting session...");
                proxy.StartSession(vehicleId);

                Console.WriteLine("Sending sample data...");
                ChargingData validData = new ChargingData
                {
                    VehicleId = vehicleId,
                    RowIndex = 1,
                    TimeStamp = DateTime.Now,
                    VoltageRMSMin = 220.0,
                    VoltageRMSAvg = 230.0,
                    VoltageRMSMax = 240.0,
                    CurrentRMSMin = 10.0,
                    CurrentRMSAvg = 15.0,
                    CurrentRMSMax = 20.0,
                    RealPowerMin = 2200.0,
                    RealPowerAvg = 3450.0,
                    RealPowerMax = 4800.0,
                    ReactivePowerMin = 100.0,
                    ReactivePowerAvg = 200.0,
                    ReactivePowerMax = 300.0,
                    ApparentPowerMin = 2250.0,
                    ApparentPowerAvg = 3500.0,
                    ApparentPowerMax = 4850.0,
                    FrequencyMin = 49.8,
                    FrequencyAvg = 50.0,
                    FrequencyMax = 50.2
                };

                proxy.PushSample(validData);
                Console.WriteLine("valid data sent successfuly");

                Console.WriteLine("testing invalid data");
                ChargingData invalidData = new ChargingData
                {
                    VehicleId = vehicleId,
                    RowIndex = 2,
                    TimeStamp = DateTime.Now,
                    VoltageRMSMin = -220.0,
                    VoltageRMSAvg = 230.0,
                    VoltageRMSMax = 240.0,
                    FrequencyMin = 49.8,
                    FrequencyAvg = 50.0,
                    FrequencyMax = 50.2
                };

                proxy.PushSample(invalidData);
                Console.WriteLine("invalid data sent");
            }
            catch(FaultException<ChargingException> ex)
            {
                Console.WriteLine($"Service error: {ex.Detail.Message}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            try
            {
                Console.WriteLine("Ending session...");
                proxy.EndSession("golf_4_001");
                Console.WriteLine("Test completed!");
            } 
            catch(FaultException<ChargingException> ex)
            {
                Console.WriteLine($"Service error: {ex.Detail.Message}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
