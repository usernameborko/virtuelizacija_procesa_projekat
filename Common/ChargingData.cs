using System;
using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class ChargingData
    {
        [DataMember]
        public DateTime TimeStamp { get; set; }

        [DataMember]
        public double VoltageRMSMin {  get; set; }

        [DataMember]
        public double VoltageRMSAvg {  get; set; }

        [DataMember]
        public double VoltageRMSMax {  get; set; }

        [DataMember]
        public double CurrentRMSMin {  get; set; }

        [DataMember]
        public double CurrentRMSAvg {  get; set; }

        [DataMember]
        public double CurrentRMSMax {  get; set; }

        [DataMember]
        public double RealPowerMin {  get; set; }

        [DataMember]
        public double RealPowerAvg {  get; set; }

        [DataMember]
        public double RealPowerMax {  get; set; }

        [DataMember]
        public double ReactivePowerMin {  get; set; }

        [DataMember]
        public double ReactivePowerAvg {  get; set; }

        [DataMember]
        public double ReactivePowerMax {  get; set; }

        [DataMember]
        public double ApparentPowerMin {  get; set; }

        [DataMember]
        public double ApparentPowerAvg {  get; set; }

        [DataMember]
        public double ApparentPowerMax {  get; set; }

        [DataMember]
        public double FrequencyMin {  get; set; }

        [DataMember]
        public double FrequencyAvg {  get; set; }

        [DataMember]
        public double FrequencyMax {  get; set; }

        [DataMember]
        public int RowIndex {  get; set; }

        [DataMember]
        public string VehicleId {  get; set; }
    }
}
