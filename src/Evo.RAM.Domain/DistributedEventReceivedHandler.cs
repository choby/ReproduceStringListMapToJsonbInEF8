using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace Evo.RAM;

public class DistributedEventReceivedHandler : ILocalEventHandler<DistributedEventReceived>, ITransientDependency
{
    public async Task HandleEventAsync(DistributedEventReceived eventData)
    {
        // TODO: IMPLEMENT YOUR LOGIC...
    }
}