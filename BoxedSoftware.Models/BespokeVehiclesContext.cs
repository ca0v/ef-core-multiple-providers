using Bogus;
using Microsoft.EntityFrameworkCore;

namespace BoxedSoftware.Models;

public partial class VehiclesContext : DbContext
{
    public static async Task InitializeAsync(VehiclesContext db)
    {
        await db.Database.MigrateAsync();

        // already seeded
        if (db.Vehicles.Any())
            return;

        // sample data will be different due
        // to the nature of generating data
        var fake = new Faker<Vehicle>()
            .Rules((f, v) => v.VehicleIdentificationNumber = f.Vehicle.Vin())
            .Rules((f, v) => v.Model = f.Vehicle.Model())
            .Rules((f, v) => v.Type = f.Vehicle.Type())
            .Rules((f, v) => v.Fuel = f.Vehicle.Fuel());

        var vehicles = fake.Generate(100);

        db.Vehicles.AddRange(vehicles);
        await db.SaveChangesAsync();
    }
}
