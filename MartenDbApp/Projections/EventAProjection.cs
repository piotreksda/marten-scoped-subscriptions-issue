using Marten.Events.Aggregation;
using MartenDbApp.Events;

namespace MartenDbApp.Projections;

public class EventAProjection : SingleStreamProjection<EventAReadModel>
{
    public EventAProjection()
    {
        ProjectEvent<EventA>((item, @event) =>
        {
            item.Id = Guid.NewGuid();
            item.Test = @event.Test;
        });
    }
}