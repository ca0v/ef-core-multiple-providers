using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BoxedSoftware.Models;

public partial class VehiclesContext : DbContext
{
    public VehiclesContext()
    {
    }

    public VehiclesContext(DbContextOptions<VehiclesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
