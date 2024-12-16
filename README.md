# Multiple Database Providers with EF Core

If you need the same models and relationships across multiple database providers, well then, this is the repository for you.

The goal of this repository is to show how you can set up a solution to have one `DbContext` interface to multiple providers. In this case, we'll be using our `VehiclesContext` to communicate with SQLite and MySQL.

## Prerequisites

- .NET 8
- MySQL instance

## Provider Switching

Provider switching happens in the host application. In this example, we're using a argument passed in at the start of the application lifetime. Since we're using the `IConfiguration` instance, we could also set this value using environment variables, JSON values, or through any other registered configuration provider.

```csharp
builder.Services.AddDbContext<VehiclesContext>(options =>
{
    var provider = config.GetValue("provider", Sqlite.Name);

    if (provider == Sqlite.Name)
    {
        options.UseSqlite(
            config.GetConnectionString(Sqlite.Name)!,
            x => x.MigrationsAssembly(Sqlite.Assembly)
        );
    }

    if (provider == Postgres.Name) {
        options.UseNpgsql(
            config.GetConnectionString(Postgres.Name)!,
            x => x.MigrationsAssembly(Postgres.Assembly)
        );
    }
});
```

## Commands

Commands are assumed to be running at the root of the solution directory, and you will need to adjust your paths depending on your solution and projects.

```console
> dotnet ef migrations add DemoMigration --startup-project ./BoxedSoftware --project ./Migrations/BoxedSoftware.Sqlite -- --provider Sqlite

> dotnet ef migrations add DemoMigration --startup-project ./BoxedSoftware --project ./Migrations/BoxedSoftware.Postgres -- --provider Postgres
```

## Projects

This sample includes a `Host`, `Models`, and one project for each database provider. That's a total of 4 projects.
