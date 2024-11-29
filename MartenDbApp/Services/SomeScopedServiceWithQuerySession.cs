using Marten;
using MartenDbApp.Projections;

namespace MartenDbApp;

public class SomeScopedServiceWithQuerySession(IQuerySession querySession)
{
    public async Task<bool> AnyDocumentExist()
    {
        return await querySession.Query<EventAReadModel>().AnyAsync();
    }
}