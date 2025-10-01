using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class ChargingException
    {
        [DataMember]
        public string Message { get; set; }

        public ChargingException(string message)
        {
            this.Message = message;
        }
    }
}
