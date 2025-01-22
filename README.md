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

### Create BoxedSoftware.AzureSql Project

To create a new class library project in a subfolder, you can use the `dotnet new classlib` command followed by the `-o` option to specify the output directory. For example:

```console
> dotnet new classlib -n BoxedSoftware.AzureSql -o ./Migrations/BoxedSoftware.AzureSql
> dotnet add ./Migrations/BoxedSoftware.AzureSql/BoxedSoftware.AzureSql.csproj reference BoxedSoftware.Models/BoxedSoftware.Models.csproj
> dotnet add ./Migrations/BoxedSoftware.AzureSql/BoxedSoftware.AzureSql.csproj package Microsoft.EntityFrameworkCore.Design
> dotnet add ./Migrations/BoxedSoftware.AzureSql/BoxedSoftware.AzureSql.csproj package Microsoft.EntityFrameworkCore.Relational
> dotnet add ./Migrations/BoxedSoftware.AzureSql/BoxedSoftware.AzureSql.csproj package Microsoft.EntityFrameworkCore.SqlServer
```

Note that if you want to install 8.* versions of these packages, do it like this:

 > dotnet add ./Migrations/BoxedSoftware.AzureSql/BoxedSoftware.AzureSql.csproj package Microsoft.EntityFrameworkCore.Design --version 8.0.0

To query available packages:

 > dotnet list package --outdated



Now tell the main project about this new project

```console
> dotnet add ./BoxedSoftware/BoxedSoftware.csproj reference Migrations/BoxedSoftware.AzureSql/BoxedSoftware.AzureSql.csproj
> dotnet add ./BoxedSoftware/BoxedSoftware.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
```

Finally, add the migration to the new project

```console
> dotnet ef migrations add DemoMigration --startup-project ./BoxedSoftware --project ./Migrations/BoxedSoftware.AzureSql -- --provider SqlServer
```

```console
> dotnet ef migrations add DemoMigration --startup-project ./BoxedSoftware --project ./Migrations/BoxedSoftware.Sqlite -- --provider Sqlite

> dotnet ef migrations add DemoMigration --startup-project ./BoxedSoftware --project ./Migrations/BoxedSoftware.Postgres -- --provider Postgres
```

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
