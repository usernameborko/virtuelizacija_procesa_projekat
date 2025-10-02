using System;
using System.Collections.Generic;
using System.Globalization;
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
                CultureInfo culture = CultureInfo.InvariantCulture;

                for(int i=0; i<10; i++)
                {
                    DateTime timestamp = baseTime.AddSeconds(i);

                    double voltageBase = 220 + rand.Next(0, 20);
                    double currentBase = 10 + rand.Next(0, 15);
                    double freqBase = 49.8 + (rand.NextDouble() * 0.4);

                    writer.WriteLine(string.Format(culture,
                        "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}",
                        timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                        voltageBase.ToString("F1", culture),
                        (voltageBase + 10).ToString("F1", culture),
                        (voltageBase + 20).ToString("F1", culture),
                        currentBase.ToString("F1", culture),
                        (currentBase + 5).ToString("F1", culture),
                        (currentBase + 10).ToString("F1", culture),
                        (voltageBase * currentBase).ToString("F0", culture),
                        ((voltageBase + 10) * (currentBase + 5)).ToString("F0", culture),
                        ((voltageBase + 20) * (currentBase + 10)).ToString("F0", culture),
                        (100 + i * 10).ToString("F0", culture),
                        (200 + i * 10).ToString("F0", culture),
                        (300 + i * 10).ToString("F0", culture),
                        ((voltageBase * currentBase) + 50).ToString("F0", culture),
                        (((voltageBase + 10) * (currentBase + 5)) + 50).ToString("F0", culture),
                        (((voltageBase + 20) * (currentBase + 10)) + 50).ToString("F0", culture),
                        freqBase.ToString("F2", culture),
                        (freqBase + 0.1).ToString("F2", culture),
                        (freqBase + 0.2).ToString("F2", culture)
                    ));
                }
            }
        }
    }
}
