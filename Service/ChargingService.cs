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
