using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IChargingService
    {
        [OperationContract]
        bool StartSession(string vehicleId);

        [OperationContract]
        bool PushSample(ChargingData data);

        [OperationContract]
        bool EndSession(string vehicleId);
    }
}
