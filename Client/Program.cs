using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace Client
{
    public class Program
    {
        private static string[] vehicleTypes = {
            "BMW_i3", "Tesla_Model_S", "Audi_A3", "Mercedes_GCS",
            "Volkswagen_MK2", "Nissan_Leaf", "Hyundai_Kona", "Kia_e-Niro",
            "Jaguar_Pace", "Porsche_Taycan", "Volvo_XC40", "Peugeot_e-208"
        };
        static void Main(string[] args)
        {
            Console.WriteLine("Generating test data...");
            TestGenerator.GenerateTestData();
            Console.WriteLine("Test data generated!\n");

            string selectedVehicle = ShowVehicleMenu();
            if (string.IsNullOrEmpty(selectedVehicle))
            {
                Console.WriteLine("No vehicle selectet.");
                return;
            }

            string csvPath = Path.Combine("VehicleData", selectedVehicle, "Charging_Profile.csv");
            Console.WriteLine($"Parsing CSV file: {csvPath}");

            List<ChargingData> dataList = CSVParser.ParseCSVFile(csvPath, selectedVehicle);
            Console.WriteLine($"Parsed {dataList.Count} records\n");

            if(dataList.Count == 0)
            {
                Console.WriteLine("No valid data to send.");
                return;
            }

            SendDataToService(dataList, selectedVehicle);

            /*
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
            */

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Console.WriteLine("Press ENTER to close...");
            Console.ReadLine();
        }

        private static string ShowVehicleMenu()
        {
            Console.WriteLine("=== SELECT VEHICLE ===");
            for(int i=0; i<vehicleTypes.Length; i++)
            {
                Console.WriteLine($"{i + 1} - {vehicleTypes[i]}");
            }
            Console.WriteLine("0 - EXIT");
            Console.Write("Choose vehicle (1 - 12): ");

            string input = Console.ReadLine();
            if(int.TryParse(input, out int choice))
            {
                if(choice >= 1 && choice <= vehicleTypes.Length)
                {
                    string selectedId = vehicleTypes[choice - 1];
                    Console.WriteLine($"Selected: {selectedId}\n");
                    return selectedId;
                } 
                else if(choice == 0)
                {
                    return null;
                }
            }

            Console.WriteLine("Invalid choice!");
            return ShowVehicleMenu();
        }

        private static void SendDataToService(List<ChargingData> dataList, string vehicleId)
        {
            ChannelFactory<IChargingService> factory = new ChannelFactory<IChargingService>("ChargingService");
            IChargingService proxy = factory.CreateChannel();

            try
            {
                Console.WriteLine("=== SENDING DATA TO SERVICE ===");

                Console.WriteLine("Starting session...");
                proxy.StartSession(vehicleId);

                int successCount = 0;
                foreach(ChargingData data in dataList)
                {
                    try
                    {
                        Console.WriteLine($"Sending row {data.RowIndex}");
                        proxy.PushSample(data);
                        successCount++;

                        System.Threading.Thread.Sleep(500);
                    }
                    catch(FaultException<ChargingException> ex)
                    {
                        Console.WriteLine($"Row {data.RowIndex} rejected: {ex.Detail.Message}");

                        if (ex.Detail.Message.Contains("interrupted"))
                        {
                            Console.WriteLine("Transfer was interrupted - resources disposed!");
                            break;
                        }
                    }
                }

                Console.WriteLine($"Successfully sent {successCount}/{dataList.Count} records");

                Console.WriteLine("Ending charging session...");
                proxy.EndSession(vehicleId);
                Console.WriteLine("Session ended successfuly");
            }
            catch (FaultException<ChargingException> ex)
            {
                Console.WriteLine($"Service error: {ex.Detail.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexplected error: {ex.Message}");
            }
            finally
            {
                try
                {
                    factory.Close();
                }
                catch
                {
                    factory.Abort();
                }
            }
        }
    }
}
