using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Events;

namespace Events
{

  public class LocationEvent : IEvent
  {
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string CorrelationId { get; set; }
    public ObjectId DeviceId { get; set; }
    public string Imei { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Date { get; set; }
    public bool? PositionStatus { get; set; }
    public float? Speed { get; set; }
    public float? Heading { get; set; }
    public float? Altitude { get; set; }
    public int? Satellites { get; set; }
    public float? HDOP { get; set; }
    public short? GsmSignal { get; set; }
    public double? Odometer { get; set; }

    public int? MobileCountryCode { get; set; }
    public int? MobileNetworkCode { get; set; }
    public int? LocationAreaCode { get; set; }
    public int? CellId { get; set; }
  }

}
