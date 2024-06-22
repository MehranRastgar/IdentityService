using System;

namespace Events
{
  public interface IEvent
  {
    DateTime Timestamp { get; }
    string CorrelationId { get; }
  }

  public class UserCreatedEvent : IEvent
  {
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string CorrelationId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    // Add other relevant data
  }

  public class UserDeletedEvent : IEvent
  {
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string CorrelationId { get; set; }
    public string UserName { get; set; }
  }

  public class UserUpdatedEvent : IEvent
  {
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string CorrelationId { get; set; }
    public string UserName { get; set; }
    // Include fields that were updated or the updated user information
  }
}