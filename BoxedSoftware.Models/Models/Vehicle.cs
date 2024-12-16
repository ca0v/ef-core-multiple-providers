using System;
using System.Collections.Generic;

namespace BoxedSoftware.Models;

public partial class Vehicle
{
    public long Id { get; set; }

    public string VehicleIdentificationNumber { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Fuel { get; set; } = null!;
}
