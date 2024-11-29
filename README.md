# MartenDbApp - issue

This is a sample application using Marten with ASP.NET Core.

## Requirements

- .NET 8.0
- PostgreSQL

## Configuration

Ensure you have a PostgreSQL instance running. The connection string is configured in `appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=marten_db_tests;Username=postgres;Password=mysecretpassword"
}
```

## The problem

when you want to use subscription which has dependency on IQuerySession or IDocumentSession app will freaze on startup
