using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Service
{
    public class ChargingService : IChargingService
    {
        private static Dictionary<string, bool> activeSessions = new Dictionary<string, bool>();
        private static Dictionary<string, FileManager> sessionFiles = new Dictionary<string, FileManager>();

        private static Dictionary<string, StreamWriter> sessionWriters = new Dictionary<string, StreamWriter>();
        private static Dictionary<string, StreamWriter> rejectWriters = new Dictionary<string, StreamWriter>();

        private static Dictionary<string, int> sessionCounters = new Dictionary<string, int>();

        public static Dictionary<string, ChargingData> lastSamples = new Dictionary<string, ChargingData>();

        public delegate void ChargigngEventHandler(object sender, ChargingEventArgs e);

        public static event ChargigngEventHandler OnTransferStarted;
        public static event ChargigngEventHandler OnSampleReceived;
        public static event ChargigngEventHandler OnTransferCompleted;
        public static event ChargigngEventHandler OnWarningRaised;

        public static event ChargigngEventHandler OnVoltageSpike;
        public static event ChargigngEventHandler OnCurrentSpike;

        private const double VOLTAGE_LIMIT = 10.0;
        private const double CURRENT_LIMIT = 5.0;

        private void RaiseTransferStarted(string vehicleId, string message)
        {
            if(OnTransferStarted != null)
            {
                OnTransferStarted(this, new ChargingEventArgs(vehicleId, message));
            }
        }
        private void RaiseSampleReceived(string vehicleId, string message, int rowIndex)
        {
            if(OnSampleReceived != null)
            {
                OnSampleReceived(this, new ChargingEventArgs(vehicleId, message, rowIndex));
            }
        }
        private void RaiseTransferCompleted(string vehicleId, string message)
        {
            if(OnTransferCompleted != null)
            {
                OnTransferCompleted(this, new ChargingEventArgs(vehicleId, message));
            }
        }
        private void RaiseWaningRaised(string vehicleId, string message, int rowIndex = 0)
        {
            if(OnWarningRaised != null)
            {
                OnWarningRaised(this, new ChargingEventArgs(vehicleId, message, rowIndex));
            }
        }

        private void RaiseVoltageSpike(string vehicleId, string message, int rowIndex = 0)
        {
            if(OnVoltageSpike != null)
            {
                OnVoltageSpike(this, new ChargingEventArgs(vehicleId, message, rowIndex));
            }
        }

        private void RaiseCurrentSpike(string vehicleId, string message, int rowIndex = 0)
        {
            if(OnCurrentSpike != null)
            {
                OnCurrentSpike(this, new ChargingEventArgs(vehicleId, message, rowIndex));
            }
        }

        private void AnalyzeVoltageAndCurrent(ChargingData currentData)
        {
            if (lastSamples.ContainsKey(currentData.VehicleId))
            {
                ChargingData lastData = lastSamples[currentData.VehicleId];

                double deltaV = Math.Abs(currentData.VoltageRMSAvg - lastData.VoltageRMSAvg);
                double deltaI = Math.Abs(currentData.CurrentRMSAvg - lastData.CurrentRMSAvg);

                if(deltaV > VOLTAGE_LIMIT)
                {
                    string message = $"Voltage spike detected: daltaV = {deltaV:F2}V (limit: {VOLTAGE_LIMIT}V)";
                    Console.WriteLine(message);
                    RaiseVoltageSpike(currentData.VehicleId, message, currentData.RowIndex);
                }

                if (deltaI > CURRENT_LIMIT)
                {
                    string message = $"Current spike detected: daltaI = {deltaI:F2}A (limit: {VOLTAGE_LIMIT}A)";
                    Console.WriteLine(message);
                    RaiseVoltageSpike(currentData.VehicleId, message, currentData.RowIndex);
                }
            }

            lastSamples[currentData.VehicleId] = currentData;
        }


        public bool StartSession(string vehicleId)
        {
            if (string.IsNullOrEmpty(vehicleId))
            {
                throw new FaultException<ChargingException>(new ChargingException("Vehicle ID cannot be empty!"));
            }

            if (activeSessions.ContainsKey(vehicleId))
            {
                throw new FaultException<ChargingException>(new ChargingException("Session already active for this vehicle"));
            }

            try
            {
                string dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
                string sessionDir = Path.Combine("Data", vehicleId, dateFolder);

                if (!Directory.Exists(sessionDir))
                {
                    Directory.CreateDirectory(sessionDir);
                }

                string sessionFilePath = Path.Combine(sessionDir, "session.csv");
                StreamWriter sessionWriter = new StreamWriter(sessionFilePath, true);

                if(new FileInfo(sessionFilePath).Length == 0)
                {
                    sessionWriter.WriteLine("Timestamp,VehicleId,RowIndex,VoltageRMSMin,VoltageRMSAvg,VoltageRMSMax,CurrentRMSMin,CurrentRMSAvg,CurrentRMSMax,RealPowerMin,RealPowerAvg,RealPowerMax,ReactivePowerMin,ReactivePowerAvg,ReactivePowerMax,ApparentPowerMin,ApparentPowerAvg,ApparentPowerMax,FrequencyMin,FrequencyAvg,FrequencyMax");
                }

                string rejectsFilePath = Path.Combine(sessionDir, "rejects.csv");
                StreamWriter rejectWriter = new StreamWriter(rejectsFilePath, true);

                if(new FileInfo(rejectsFilePath).Length == 0)
                {
                    rejectWriter.WriteLine("Timestamp,VehicleId,RowIndex,RejectReason");
                }

                activeSessions[vehicleId] = true;
                sessionWriters[vehicleId] = sessionWriter;
                rejectWriters[vehicleId] = rejectWriter;
                sessionCounters[vehicleId] = 0;

                Console.WriteLine("=== SESSION STARTED ===");
                Console.WriteLine($"Vehicle: {vehicleId}");
                Console.WriteLine($"Directory: {sessionDir}");
                Console.WriteLine($"Status: READY FOR DATA TRANSFER");
                Console.WriteLine("========================");

                RaiseTransferStarted(vehicleId, $"Session started for vehicle {vehicleId}");

                return true;

                /*
                string sessionFilePath = $"session_{vehicleId}.txt";
                FileManager fileManager = new FileManager(sessionFilePath);

                activeSessions[vehicleId] = true;
                sessionFiles[vehicleId] = fileManager;

                Console.WriteLine($"Session started for vehicle: {vehicleId}");
                Console.WriteLine($"Created file resource: {sessionFilePath}");
                return true;
                */
            }
            catch (Exception ex)
            {
                throw new FaultException<ChargingException>(new ChargingException($"Failed to start session: {ex.Message}"));
            }
        }

        public bool PushSample(ChargingData data)
        {
            if(data == null)
            {
                throw new FaultException<ChargingException>(new ChargingException("Data cannot be null"));
            }

            if (!activeSessions.ContainsKey(data.VehicleId))
            {
                throw new FaultException<ChargingException>(new ChargingException("No active session for this vehicle"));
            }

            Console.WriteLine($"TRANSFER IN PROGRESS - Vehicle: {data.VehicleId}, Row: {data.RowIndex}");

            try
            {
                ValidateData(data);

                SaveValidData(data);

                AnalyzeVoltageAndCurrent(data);

                sessionCounters[data.VehicleId]++;

                Console.WriteLine($"SAMPLE ACCEPTED AND SAVED - Total: {sessionCounters[data.VehicleId]}");

                RaiseSampleReceived(data.VehicleId, $"Sample {data.RowIndex} processed successfully", data.RowIndex);

                return true;
            }
            catch(FaultException<ChargingException> validationEx)
            {
                SaveRejectedData(data, validationEx.Detail.Message);

                Console.WriteLine($"SAMPLE REJECTED- Reason: {validationEx.Detail.Message}");

                RaiseWaningRaised(data.VehicleId, $"Sample {data.RowIndex} rejected: {validationEx.Detail.Message}", data.RowIndex);

                throw;
            }

            /*
            if(data.RowIndex % 5 == 0)
            {
                Console.WriteLine("Simulating disposing resources");
                if (sessionFiles.ContainsKey(data.VehicleId))
                {
                    sessionFiles[data.VehicleId].Dispose();
                    sessionFiles.Remove(data.VehicleId);
                }
                throw new FaultException<ChargingException>(new ChargingException("Resources disposed."));
            }

            Console.WriteLine($"Received valid sample for vehicle {data.VehicleId}, Row: {data.RowIndex}");
            return true;
            */
        }

        private void ValidateData(ChargingData data)
        {
            if (data.TimeStamp == default(DateTime) || data.TimeStamp > DateTime.Now.AddMinutes(5))
            {
                throw new FaultException<ChargingException>(new ChargingException("Date must be a valid date not in future."));
            }

            if (data.VoltageRMSMin <= 0 || data.VoltageRMSAvg <= 0 || data.VoltageRMSMax <= 0)
            {
                throw new FaultException<ChargingException>(new ChargingException("Voltage values must be greater than 0."));
            }

            if (data.FrequencyMin <= 0 || data.FrequencyAvg <= 0 || data.FrequencyMax <= 0)
            {
                throw new FaultException<ChargingException>(new ChargingException("Frequency values must be greater than 0."));
            }

            if (data.VoltageRMSMin > data.VoltageRMSAvg || data.VoltageRMSAvg > data.VoltageRMSMax)
            {
                throw new FaultException<ChargingException>(new ChargingException("Min voltage must be <= Avg <= Max."));
            }

            if (data.FrequencyMin > data.FrequencyAvg || data.FrequencyAvg > data.FrequencyMax)
            {
                throw new FaultException<ChargingException>(new ChargingException("Min frequency must be <= Avg <= Max."));
            }
        }

        private void SaveValidData(ChargingData data)
        {
            if (sessionWriters.ContainsKey(data.VehicleId))
            {
                StreamWriter writer = sessionWriters[data.VehicleId];
                string line = $"{data.TimeStamp:yyyy-MM-dd HH:mm:ss},{data.VehicleId},{data.RowIndex},{data.VoltageRMSMin},{data.VoltageRMSAvg},{data.VoltageRMSMax},{data.CurrentRMSMin},{data.CurrentRMSAvg},{data.CurrentRMSMax},{data.RealPowerMin},{data.RealPowerAvg},{data.RealPowerMax},{data.ReactivePowerMin},{data.ReactivePowerAvg},{data.ReactivePowerMax},{data.ApparentPowerMin},{data.ApparentPowerAvg},{data.ApparentPowerMax},{data.FrequencyMin},{data.FrequencyAvg},{data.FrequencyMax}";
                writer.WriteLine(line);
                writer.Flush();
            }
        }

        private void SaveRejectedData(ChargingData data, string reason)
        {
            if (rejectWriters.ContainsKey(data.VehicleId))
            {
                StreamWriter writer = rejectWriters[data.VehicleId];
                string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}, {data.VehicleId}. {reason}";
                writer.WriteLine(line);
                writer.Flush();
            }
        }

        public bool EndSession(string vehicleId)
        {
            if (!activeSessions.ContainsKey(vehicleId))
            {
                throw new FaultException<ChargingException>(new ChargingException("No active session for this vehicle"));
            }

            try
            {
                int total = sessionCounters.ContainsKey(vehicleId) ? sessionCounters[vehicleId] : 0;

                if (sessionWriters.ContainsKey(vehicleId))
                {
                    sessionWriters[vehicleId].Close();
                    sessionWriters[vehicleId].Dispose();
                    sessionWriters.Remove(vehicleId);
                }

                if (rejectWriters.ContainsKey(vehicleId))
                {
                    rejectWriters[vehicleId].Close();
                    rejectWriters[vehicleId].Dispose();
                    rejectWriters.Remove(vehicleId);
                }

                activeSessions.Remove(vehicleId);
                sessionCounters.Remove(vehicleId);

                if (lastSamples.ContainsKey(vehicleId))
                {
                    lastSamples.Remove(vehicleId);
                }

                Console.WriteLine("=== TRANSFER COMPLETE ===");
                Console.WriteLine($"Vehicle: {vehicleId}");
                Console.WriteLine($"Total records processed: {total}");
                Console.WriteLine($"Status: SESSION CLOSED");
                Console.WriteLine("========================");

                RaiseTransferCompleted(vehicleId, $"Transfer completed for vehicle {vehicleId}. Total record: {total}");

                return true;
            }
            catch(Exception ex)
            {
                throw new FaultException<ChargingException>(new ChargingException($"Error ending session: {ex.Message}"));
            }

            /*
            if (sessionFiles.ContainsKey(vehicleId))
            {
                sessionFiles[vehicleId].Dispose();
                sessionFiles.Remove(vehicleId);
                Console.WriteLine($"Resources properly disposed for vehicle: {vehicleId}");
            }

            activeSessions.Remove(vehicleId);
            Console.WriteLine($"Session ended for vehicle: {vehicleId}");
            return true;
            */
        }

    }
}
