using Common;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Service
{
    public class ChargingService : IChargingService
    {
        private static Dictionary<string, bool> activeSessions = new Dictionary<string, bool>();

        public bool EndSession(string vehicleId)
        {
            if (!activeSessions.ContainsKey(vehicleId))
            {
                throw new FaultException<ChargingException>(new ChargingException("No active session for this vehicle"));
            }

            activeSessions.Remove(vehicleId);
            Console.WriteLine($"Session ended for vehicle: {vehicleId}");
            return true;
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

            if(data.TimeStamp == default(DateTime) || data.TimeStamp > DateTime.Now.AddMinutes(5))
            {
                throw new FaultException<ChargingException>(new ChargingException("Date must be a valid date not in future."));
            }

            if(data.VoltageRMSMin <= 0 || data.VoltageRMSAvg <= 0 || data.VoltageRMSMax <= 0)
            {
                throw new FaultException<ChargingException>(new ChargingException("Voltage values must be greater than 0."));
            }

            if(data.FrequencyMin <= 0 || data.FrequencyAvg <= 0 || data.FrequencyMax <= 0)
            {
                throw new FaultException<ChargingException>(new ChargingException("Frequency values must be greater than 0."));
            }

            if(data.VoltageRMSMin > data.VoltageRMSAvg || data.VoltageRMSAvg > data.VoltageRMSMax)
            {
                throw new FaultException<ChargingException>(new ChargingException("Min voltage must be <= Avg <= Max."));
            }

            if(data.FrequencyMin > data.FrequencyAvg || data.FrequencyAvg > data.FrequencyMax)
            {
                throw new FaultException<ChargingException>(new ChargingException("Min frequency must be <= Avg <= Max."));
            }

            Console.WriteLine($"Recieved sample for vehicle {data.VehicleId}, Row: {data.RowIndex}");
            return true;
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

            activeSessions[vehicleId] = true;
            Console.WriteLine($"Session started for vehicle: {vehicleId}");
            return true;
        }
    }
}
