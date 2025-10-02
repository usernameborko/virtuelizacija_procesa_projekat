using Common;
using System;
using System.ServiceModel;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Generating test data...");
            TestGenerator.GenerateTestData();
            Console.WriteLine("Test data generated!\n");

            ChannelFactory<IChargingService> factory = new ChannelFactory<IChargingService>("ChargingService");
            IChargingService proxy = factory.CreateChannel();

            try
            {
                string vehicleId = "golf_4_001";

                Console.WriteLine("Starting session...");
                proxy.StartSession(vehicleId);

                for(int i=1; i <= 7; i++)
                {
                    try
                    {
                        Console.WriteLine($"Sending sample {i}...");
                        ChargingData data = new ChargingData
                        {
                            VehicleId = vehicleId,
                            RowIndex = i,
                            TimeStamp = DateTime.Now,
                            VoltageRMSMin = 220.0 + i,
                            VoltageRMSAvg = 230.0 + i,
                            VoltageRMSMax = 240.0 + i,
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

                        proxy.PushSample(data);
                        Console.WriteLine($"Sample {i} sent successfuly");

                    }
                    catch (FaultException<ChargingException> ex)
                    {
                        Console.WriteLine($"Sample {i} failed: {ex.Detail.Message}");

                        if (ex.Detail.Message.Contains("interrupted"))
                        {
                            Console.WriteLine("Transfer was interupted - resources disposed!");
                            break;
                        }
                    }
                }

            }
            catch(FaultException<ChargingException> ex)
            {
                Console.WriteLine($"Service error: {ex.Detail.Message}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
