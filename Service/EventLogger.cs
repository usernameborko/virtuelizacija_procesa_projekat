using Common;
using System;

namespace Service
{
    public class EventLogger
    {
        public void OnTransferStarted(object sender, ChargingEventArgs e)
        {
            Console.WriteLine($"[EVENT] TRANSFER STARTED: {e.Message} at {e.Timestamp}");
        }

        public void OnSampleReceived(object sender, ChargingEventArgs e)
        {
            Console.WriteLine($"[EVENT] SAMPLE RECEIVED: {e.Message} at {e.Timestamp}");
        }

        public void OnTransferCompleted(object sender, ChargingEventArgs e)
        {
            Console.WriteLine($"[EVENT] TRANSFER COMPLETED: {e.Message} at {e.Timestamp}");
        }
        public void OnWarningRaised(object sender, ChargingEventArgs e)
        {
            Console.WriteLine($"[WARNING] {e.Message} at {e.Timestamp}");
        }
    }
}
