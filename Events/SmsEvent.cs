using System;
using System.Runtime.InteropServices;
using MassTransit;


namespace SmsEvents
{

    public interface IEvent
    {
        DateTime Timestamp { get; }
        string CorrelationId { get; }
    }

    public class SmsEvent : IEvent
    {
        public required string Message { get; set; }
        public required string[] PhoneNumbers { get; set; }
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; }

    }

}