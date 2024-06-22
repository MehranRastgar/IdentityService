using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Events;

namespace Events
{

  public class StatusEvent : IEvent
  {
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string CorrelationId { get; set; }
    public ObjectId DeviceId { get; set; }
    public string Imei { get; set; }
    public string? VoltageLevel { get; set; }        // Added for voltage level
    public string? GsmSignalStrength { get; set; }
    public bool? Status { get; set; }
    public bool? Ignition { get; set; }
    public bool? Charging { get; set; }
    public string? AlarmType { get; set; }
    public bool? GpsTracking { get; set; }
    public bool? RelayState { get; set; }
  }

}


