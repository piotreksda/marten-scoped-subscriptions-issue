using Marten;
using Marten.Events.Daemon;
using Marten.Events.Daemon.Internals;
using Marten.Subscriptions;

namespace MartenDbApp.Subscriptions;

public class NotWorkingTestSubscription : SubscriptionBase
{
    private readonly SomeScopedServiceWithQuerySession _someScopedServiceWithQuerySession;
    public NotWorkingTestSubscription(SomeScopedServiceWithQuerySession someScopedServiceWithQuerySession)
    {
        SubscriptionName = "WorkingTestSubscription";
        Options.BatchSize = 1;
        Options.SubscribeFromSequence(0);
        
        _someScopedServiceWithQuerySession = someScopedServiceWithQuerySession;
    }

    public override async Task<IChangeListener> ProcessEventsAsync(EventRange page, ISubscriptionController controller, IDocumentOperations operations,
        CancellationToken cancellationToken)
    {
        var exist = await _someScopedServiceWithQuerySession.AnyDocumentExist();
        
        foreach (var @event in page.Events)
        {
            Console.WriteLine(@event.Id);
            Console.WriteLine(exist);
        }
        
        return NullChangeListener.Instance;
    }
}