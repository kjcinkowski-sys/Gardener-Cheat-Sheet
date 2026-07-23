using GardenerCheatSheet.Domain.Garden;
using GardenerCheatSheet.Domain.Plants;
using Microsoft.EntityFrameworkCore;

namespace GardenerCheatSheet.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Plant> Plants => Set<Plant>();

    public DbSet<GardenEntry> GardenEntries => Set<GardenEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Plant>(plant =>
        {
            plant.HasKey(p => p.Id);

            // TrefleId is unique only among Trefle-sourced plants; custom plants
            // store NULL, and many nulls must be allowed, so the unique index is
            // filtered to non-null values (a partial index in SQLite).
            plant.HasIndex(p => p.TrefleId)
                .IsUnique()
                .HasFilter("\"TrefleId\" IS NOT NULL");

            plant.Property(p => p.ScientificName).IsRequired();
            plant.Property(p => p.Source).HasConversion<int>();

            // Computed, derived-only properties are not persisted.
            plant.Ignore(p => p.LightRequirement);
            plant.Ignore(p => p.DisplayName);

            // GrowthInfo is a value object stored in the same table as owned columns.
            plant.OwnsOne(p => p.Growth);
            plant.Navigation(p => p.Growth).IsRequired();
        });

        modelBuilder.Entity<GardenEntry>(entry =>
        {
            entry.HasKey(e => e.Id);

            // Computed override-resolution properties are not persisted.
            entry.Ignore(e => e.IsIndoor);
            entry.Ignore(e => e.ImageUrl);

            entry.HasOne(e => e.Plant)
                .WithMany()
                .HasForeignKey(e => e.PlantId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
