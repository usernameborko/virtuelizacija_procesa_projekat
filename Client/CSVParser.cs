using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class CSVParser
    {
        public static List<ChargingData> ParseCSVFile(string filePath, string vehicleId)
        {
            List<ChargingData> dataList = new List<ChargingData>();
            List<string> errorLog = new List<string>();

            try
            {
                using(StreamReader reader = new StreamReader(filePath))
                {
                    string header = reader.ReadLine();
                    int rowIndex = 1;

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }
                        try
                        {
                            ChargingData data = ParseCSVLine(line, vehicleId, rowIndex);
                            dataList.Add(data);
                        }
                        catch (Exception ex)
                        {
                            string error = $"Row {rowIndex}: {ex.Message}";
                            errorLog.Add(error);
                            Console.WriteLine($"Error parsing row {rowIndex}: {ex.Message}");
                        }

                        rowIndex++;
                    }
                }

                if(errorLog.Count > 0)
                {
                    LogErrors(vehicleId, errorLog);
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading this file {filePath}: {ex.Message}");
            }

            return dataList;
        }

        private static ChargingData ParseCSVLine(string line, string vehicleId, int rowIndex)
        {
            string[] fields = line.Split(',');

            if(fields.Length != 19)
            {
                throw new FormatException($"Expected 19 fields, got {fields.Length}");
            }

            CultureInfo culture = CultureInfo.InvariantCulture;

            ChargingData data = new ChargingData
            {
                VehicleId = vehicleId,
                RowIndex = rowIndex,
                TimeStamp = DateTime.ParseExact(fields[0], "yyyy-MM-dd HH:ss", culture),
                VoltageRMSMin = double.Parse(fields[1], culture),
                VoltageRMSAvg = double.Parse(fields[2], culture),
                VoltageRMSMax = double.Parse(fields[3], culture),
                CurrentRMSMin = double.Parse(fields[4], culture),
                CurrentRMSAvg = double.Parse(fields[5], culture),
                CurrentRMSMax = double.Parse(fields[6], culture),
                RealPowerMin = double.Parse(fields[7], culture),
                RealPowerAvg = double.Parse(fields[8], culture),
                RealPowerMax = double.Parse(fields[9], culture),
                ReactivePowerMin = double.Parse(fields[10], culture),
                ReactivePowerAvg = double.Parse(fields[11], culture),
                ReactivePowerMax = double.Parse(fields[12], culture),
                ApparentPowerMin = double.Parse(fields[13], culture),
                ApparentPowerAvg = double.Parse(fields[14], culture),
                ApparentPowerMax = double.Parse(fields[15], culture),
                FrequencyMin = double.Parse(fields[16], culture),
                FrequencyAvg = double.Parse(fields[17], culture),
                FrequencyMax = double.Parse(fields[18], culture),
            };

            return data;
        }

        private static void LogErrors(string vehicleId, List<string> errors)
        {
            string logPath = $"pars_errors_{vehicleId}.log";
            try
            {
                using(StreamWriter writer = new StreamWriter(logPath))
                {
                    writer.WriteLine($"Parse errors for {vehicleId} - {DateTime.Now}");
                    foreach(string error in errors)
                    {
                        writer.WriteLine(error);
                    }
                }

                Console.WriteLine($"Parse errors logged to {logPath}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to write error log: {ex.Message}");
            }
        }
    }
}
