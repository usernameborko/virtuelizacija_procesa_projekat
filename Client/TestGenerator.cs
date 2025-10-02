using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class TestGenerator
    {
        private static string[] vehicleTypes = {
            "BMW_i3", "Tesla_Model_S", "Audi_A3", "Mercedes_GCS",
            "Volkswagen_MK2", "Nissan_Leaf", "Hyundai_Kona", "Kia_e-Niro",
            "Jaguar_Pace", "Porsche_Taycan", "Volvo_XC40", "Peugeot_e-208"
        };

        public static void GenerateTestData()
        {
            string baseDir = "VehicleData";

            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }

            foreach(string vehicle in vehicleTypes)
            {
                string vehicleDir = Path.Combine(baseDir, vehicle);

                if (!Directory.Exists(vehicleDir))
                {
                    Directory.CreateDirectory(vehicleDir);
                }

                string csvPath = Path.Combine(vehicleDir, "Charging_Profile.csv");
                CreateCSVFile(csvPath, vehicle);

                Console.WriteLine($"Created: {csvPath}");
            }
        }

        private static void CreateCSVFile(string filePath, string vehicleType)
        {
            Random rand = new Random();

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Timestamp,VoltageRMSMin,VoltageRMSAvg,VoltageRMSMax,CurrentRMSMin,CurrentRMSAvg,CurrentRMSMax,RealPowerMin,RealPowerAvg,RealPowerMax,ReactivePowerMin,ReactivePowerAvg,ReactivePowerMax,ApparentPowerMin,ApparentPowerAvg,ApparentPowerMax,FrequencyMin,FrequencyAvg,FrequencyMax");

                DateTime baseTime = new DateTime(2024, 1, 1, 10, 0, 0);

                for(int i=0; i<10; i++)
                {
                    DateTime timestamp = baseTime.AddSeconds(i);

                    double voltageBase = 220 + rand.Next(0, 20);
                    double currentBase = 10 + rand.Next(0, 15);
                    double freqBase = 49.8 + (rand.NextDouble() * 0.4);

                    string line = string.Format("{0:yyyy-MM-dd HH:mm:ss},{1:F1},{2:F1},{3:F1},{4:F1},{5:F1},{6:F1},{7:F0},{8:F0},{9:F0},{10:F0},{11:F0},{12:F0},{13:F0},{14:F0},{15:F0},{16:F2},{17:F2},{18:F2}",
                        timestamp,
                        voltageBase, voltageBase + 10, voltageBase + 20,
                        currentBase, currentBase + 5, currentBase + 10,
                        voltageBase * currentBase, (voltageBase + 10) * (currentBase + 5), (voltageBase + 20) * (currentBase + 10),
                        100 + i * 10, 200 + i * 10, 300 + i * 10,
                        (voltageBase * currentBase) + 50, ((voltageBase + 10) * (currentBase + 5)) + 50, ((voltageBase + 20) * (currentBase + 10)) + 50,
                        freqBase, freqBase + 0.1, freqBase + 0.2);
                    writer.WriteLine(line);
                }
            }
        }
    }
}
