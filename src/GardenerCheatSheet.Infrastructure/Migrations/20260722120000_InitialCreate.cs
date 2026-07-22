using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenerCheatSheet.Infrastructure.Migrations;

/// <summary>
/// Initial schema: the local Plant cache (Trefle-sourced or user-custom) and the
/// user's GardenEntries. TrefleId is nullable with a filtered-unique index so
/// custom plants (NULL TrefleId) can coexist. Entries carry their own photo,
/// watering and indoor overrides.
/// </summary>
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Plants",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                TrefleId = table.Column<int>(type: "INTEGER", nullable: true),
                Source = table.Column<int>(type: "INTEGER", nullable: false),
                ScientificName = table.Column<string>(type: "TEXT", nullable: false),
                CommonName = table.Column<string>(type: "TEXT", nullable: true),
                Family = table.Column<string>(type: "TEXT", nullable: true),
                ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                IsIndoor = table.Column<bool>(type: "INTEGER", nullable: false),
                Growth_Light = table.Column<int>(type: "INTEGER", nullable: true),
                Growth_AtmosphericHumidity = table.Column<int>(type: "INTEGER", nullable: true),
                Growth_MinimumPrecipitationMm = table.Column<int>(type: "INTEGER", nullable: true),
                Growth_MaximumPrecipitationMm = table.Column<int>(type: "INTEGER", nullable: true),
                Growth_SoilTexture = table.Column<string>(type: "TEXT", nullable: true),
                Growth_Description = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Plants", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GardenEntries",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                PlantId = table.Column<int>(type: "INTEGER", nullable: false),
                Nickname = table.Column<string>(type: "TEXT", nullable: true),
                DateAdded = table.Column<DateOnly>(type: "TEXT", nullable: false),
                LastWatered = table.Column<DateOnly>(type: "TEXT", nullable: true),
                Notes = table.Column<string>(type: "TEXT", nullable: true),
                ImageUrlOverride = table.Column<string>(type: "TEXT", nullable: true),
                WateringOverrideDays = table.Column<int>(type: "INTEGER", nullable: true),
                IsIndoorOverride = table.Column<bool>(type: "INTEGER", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GardenEntries", x => x.Id);
                table.ForeignKey(
                    name: "FK_GardenEntries_Plants_PlantId",
                    column: x => x.PlantId,
                    principalTable: "Plants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Plants_TrefleId",
            table: "Plants",
            column: "TrefleId",
            unique: true,
            filter: "\"TrefleId\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_GardenEntries_PlantId",
            table: "GardenEntries",
            column: "PlantId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "GardenEntries");
        migrationBuilder.DropTable(name: "Plants");
    }
}
