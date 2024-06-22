using System.Threading.Tasks;

namespace FmsShared.Library.Events;

public interface IEventHandler<in T>
{
    Task Handle(T payload);
}