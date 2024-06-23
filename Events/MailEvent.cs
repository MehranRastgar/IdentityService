using System;
using MassTransit;


namespace MailEvents
{

    public interface IEvent
    {
        DateTime Timestamp { get; }
        string CorrelationId { get; }
    }

    public class MailMessage : IEvent
    {
        public required string Receiver { get; set; }
        public required string Title { get; set; }
        public required string data { get; set; }
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; }

    }
}