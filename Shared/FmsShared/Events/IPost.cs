using System.Threading.Tasks;

namespace FmsShared.Library.Events;

public interface IPost
{
    Task Send<T>(T payload);
}