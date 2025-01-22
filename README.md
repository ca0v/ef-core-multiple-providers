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

    if (provider == Postgres.Name) {
        options.UseNpgsql(
            config.GetConnectionString(Postgres.Name)!,
            x => x.MigrationsAssembly(Postgres.Assembly)
        );
    } else {
        options.UseSqlite(
            config.GetConnectionString(Sqlite.Name)!,
            x => x.MigrationsAssembly(Sqlite.Assembly)
        );
    }
});
```

## Commands

Commands are assumed to be running at the root of the solution directory, and you will need to adjust your paths depending on your solution and projects.

Be sure to update the connection strings in [app settings](./BoxedSoftware/appsettings.json) before running any commands.

### Generate Migrations

For projects already defined, you only need to generate the initial migration:

`dotnet ef migrations add DemoMigration --startup-project ./BoxedSoftware --project ./Migrations/BoxedSoftware.Sqlite -- --provider Sqlite`

`dotnet ef migrations add DemoMigration --startup-project ./BoxedSoftware --project ./Migrations/BoxedSoftware.Postgres -- --provider Postgres`

### Create BoxedSoftware.AzureSql Project

When adding support for a new database platform, you can use the `dotnet` CLI to create a new class library for migrations:


`dotnet new classlib -n BoxedSoftware.AzureSql -o ./Migrations/BoxedSoftware.AzureSql`

`dotnet add ./Migrations/BoxedSoftware.AzureSql/BoxedSoftware.AzureSql.csproj reference BoxedSoftware.Models/BoxedSoftware.Models.csproj`

`dotnet add ./Migrations/BoxedSoftware.AzureSql/BoxedSoftware.AzureSql.csproj package Microsoft.EntityFrameworkCore.Design`

`dotnet add ./Migrations/BoxedSoftware.AzureSql/BoxedSoftware.AzureSql.csproj package Microsoft.EntityFrameworkCore.Relational`


`dotnet add ./Migrations/BoxedSoftware.AzureSql/BoxedSoftware.AzureSql.csproj package Microsoft.EntityFrameworkCore.SqlServer`


Now tell the main project about this new migration project as well as its dependencies:

`dotnet add ./BoxedSoftware/BoxedSoftware.csproj reference Migrations/BoxedSoftware.AzureSql/BoxedSoftware.AzureSql.csproj`

`dotnet add ./BoxedSoftware/BoxedSoftware.csproj package Microsoft.EntityFrameworkCore.SqlServer`

Finally, add the initial migration to the new project:

`dotnet ef migrations add DemoMigration --startup-project ./BoxedSoftware --project ./Migrations/BoxedSoftware.AzureSql -- --provider AzureSql`

## Dependencies

- Uses [Bogus](https://github.com/bchavez/Bogus) and [Faker](https://github.com/Kuree/Faker.Net), both interesting ports.

## Related Scenarios

Suppose you make changes to the SqLite database and you want to apply those changes to the Postgres database. In that case you can re-generate the models from SqLite and then re-generate both migrations against the latest models:

- ```console
  > dotnet ef dbcontext scaffold -p ./BoxedSoftware "Data Source=vehicles.db" Microsoft.EntityFrameworkCore.Sqlite -n BoxedSoftware.Models -o ../BoxedSoftware.Models/Models -c VehiclesContext -f
  ```

Now generate the new migration for both providers as you did before:

- ```console
  > dotnet ef migrations add DemoMigration2 --startup-project ./BoxedSoftware --project ./Migrations/BoxedSoftware.Sqlite -- --provider Sqlite

  > dotnet ef migrations add DemoMigration2 --startup-project ./BoxedSoftware --project ./Migrations/BoxedSoftware.Postgres -- --provider Postgres
  ```

Note that there was a slight change when I did this with the sample data. The Vehicle model changed from an int to a long.
