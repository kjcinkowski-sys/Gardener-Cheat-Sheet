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
            plant.HasIndex(p => p.TrefleId).IsUnique();
            plant.Property(p => p.ScientificName).IsRequired();

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

            // Computed override-resolution property is not persisted.
            entry.Ignore(e => e.IsIndoor);

            entry.HasOne(e => e.Plant)
                .WithMany()
                .HasForeignKey(e => e.PlantId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
