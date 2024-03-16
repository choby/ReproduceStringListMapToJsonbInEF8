using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace Evo.RAM;

public class DistributedEventSentHandler : ILocalEventHandler<DistributedEventSent>, ITransientDependency
{
    public DistributedEventSentHandler()
    {
    }

    public async Task HandleEventAsync(DistributedEventSent eventData)
    {
        
        // TODO: IMPLEMENT YOUR LOGIC...
    }
}