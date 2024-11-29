using Marten;
using Marten.Events.Daemon;
using Marten.Events.Daemon.Internals;
using Marten.Subscriptions;

namespace MartenDbApp.Subscriptions;

public class WorkingTestSubscription : SubscriptionBase
{
    public WorkingTestSubscription()
    {
        SubscriptionName = "WorkingTestSubscription";
        Options.BatchSize = 1;
        Options.SubscribeFromSequence(0);
    }
    
    public override async Task<IChangeListener> ProcessEventsAsync(EventRange page, ISubscriptionController controller, IDocumentOperations operations,
        CancellationToken cancellationToken)
    {
        foreach (var @event in page.Events)
        {
            Console.WriteLine(@event.Id);
        }
        
        return NullChangeListener.Instance;
    }
}