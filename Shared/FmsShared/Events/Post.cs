using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FmsShared.Library.DI;

namespace FmsShared.Library.Events;

[Service(typeof(IPost))]
public class Post(IServiceProvider provider) : IPost
{
    public Task Send<T>(T payload)
    {
        IEnumerable<IEventHandler<T>> eventHandlers = provider.GetServices<IEventHandler<T>>();
        
        return Task.WhenAll(eventHandlers.Select(x => x.Handle(payload)));
    }
}