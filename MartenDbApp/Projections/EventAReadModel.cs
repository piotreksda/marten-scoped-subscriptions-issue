namespace MartenDbApp.Projections;

public sealed record EventAReadModel
{
    public Guid Id { get; set; }
    public string Test { get; set; }
}