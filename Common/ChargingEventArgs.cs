using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class ChargingEventArgs
    {
        [DataMember]
        public string VehicleId {  get; set; }

        [DataMember]
        public string Message {  get; set; }

        [DataMember]
        public DateTime Timestamp {  get; set; }

        [DataMember]
        public int RowIndex { get; set; }

        public ChargingEventArgs(string vehicleId, string message, int rowIndex = 0)
        {
            VehicleId = vehicleId;
            Message = message;
            Timestamp = DateTime.Now;
            RowIndex = rowIndex;
        }

        public ChargingEventArgs() { }
    }
}
