using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GardenerCheatSheet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrefleId = table.Column<int>(type: "integer", nullable: true),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    ScientificName = table.Column<string>(type: "text", nullable: false),
                    CommonName = table.Column<string>(type: "text", nullable: true),
                    Family = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Growth_Light = table.Column<int>(type: "integer", nullable: true),
                    Growth_AtmosphericHumidity = table.Column<int>(type: "integer", nullable: true),
                    Growth_MinimumPrecipitationMm = table.Column<int>(type: "integer", nullable: true),
                    Growth_MaximumPrecipitationMm = table.Column<int>(type: "integer", nullable: true),
                    Growth_SoilTexture = table.Column<string>(type: "text", nullable: true),
                    Growth_Description = table.Column<string>(type: "text", nullable: true),
                    IsIndoor = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GardenEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlantId = table.Column<int>(type: "integer", nullable: false),
                    Nickname = table.Column<string>(type: "text", nullable: true),
                    DateAdded = table.Column<DateOnly>(type: "date", nullable: false),
                    LastWatered = table.Column<DateOnly>(type: "date", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    ImageUrlOverride = table.Column<string>(type: "text", nullable: true),
                    WateringOverrideDays = table.Column<int>(type: "integer", nullable: true),
                    IsIndoorOverride = table.Column<bool>(type: "boolean", nullable: true)
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
                name: "IX_GardenEntries_PlantId",
                table: "GardenEntries",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_TrefleId",
                table: "Plants",
                column: "TrefleId",
                unique: true,
                filter: "\"TrefleId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GardenEntries");

            migrationBuilder.DropTable(
                name: "Plants");
        }
    }
}
