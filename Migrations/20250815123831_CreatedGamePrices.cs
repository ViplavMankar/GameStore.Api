using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GameStore.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreatedGamePrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GamePrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    PricePaise = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    EffectiveFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EffectiveTo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamePrices_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 15, 12, 38, 25, 138, DateTimeKind.Utc).AddTicks(878), new DateTime(2025, 8, 15, 12, 38, 25, 138, DateTimeKind.Utc).AddTicks(878) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 15, 12, 38, 25, 138, DateTimeKind.Utc).AddTicks(878), new DateTime(2025, 8, 15, 12, 38, 25, 138, DateTimeKind.Utc).AddTicks(878) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 15, 12, 38, 25, 138, DateTimeKind.Utc).AddTicks(878), new DateTime(2025, 8, 15, 12, 38, 25, 138, DateTimeKind.Utc).AddTicks(878) });

            migrationBuilder.CreateIndex(
                name: "IX_GamePrices_GameId",
                table: "GamePrices",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GamePrices_GameId_Currency",
                table: "GamePrices",
                columns: new[] { "GameId", "Currency" });

            migrationBuilder.CreateIndex(
                name: "IX_GamePrices_GameId_Currency_IsActive",
                table: "GamePrices",
                columns: new[] { "GameId", "Currency", "IsActive" },
                unique: true,
                filter: "\"IsActive\" = TRUE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GamePrices");

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 29, 5, 29, 12, 56, DateTimeKind.Utc).AddTicks(1468), new DateTime(2025, 7, 29, 5, 29, 12, 56, DateTimeKind.Utc).AddTicks(1468) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 29, 5, 29, 12, 56, DateTimeKind.Utc).AddTicks(1468), new DateTime(2025, 7, 29, 5, 29, 12, 56, DateTimeKind.Utc).AddTicks(1468) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 29, 5, 29, 12, 56, DateTimeKind.Utc).AddTicks(1468), new DateTime(2025, 7, 29, 5, 29, 12, 56, DateTimeKind.Utc).AddTicks(1468) });
        }
    }
}
