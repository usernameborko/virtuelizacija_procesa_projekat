using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IChargingService
    {
        [OperationContract]
        [FaultContract(typeof(ChargingException))]
        bool StartSession(string vehicleId);

        [OperationContract]
        [FaultContract(typeof(ChargingException))]
        bool PushSample(ChargingData data);

        [OperationContract]
        [FaultContract(typeof(ChargingException))]
        bool EndSession(string vehicleId);
    }
}
