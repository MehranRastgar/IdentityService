using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Events;

namespace Events
{

  public class GeoCheckEventBase : IEvent
  {

    public string Id { get; set; }
    public object ActionData { get; set; }
    public string ActionerInfo { get; set; }
    public string ActionType { get; set; }
    public long FiredAt { get; set; }
    public bool IsSeen { get; set; }

    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string CorrelationId { get; set; }
  }

}
