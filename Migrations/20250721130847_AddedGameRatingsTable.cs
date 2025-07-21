using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedGameRatingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameRatings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameRatings_Games_UserId",
                        column: x => x.UserId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 21, 13, 8, 45, 578, DateTimeKind.Utc).AddTicks(18), new DateTime(2025, 7, 21, 13, 8, 45, 578, DateTimeKind.Utc).AddTicks(18) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 21, 13, 8, 45, 578, DateTimeKind.Utc).AddTicks(18), new DateTime(2025, 7, 21, 13, 8, 45, 578, DateTimeKind.Utc).AddTicks(18) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 21, 13, 8, 45, 578, DateTimeKind.Utc).AddTicks(18), new DateTime(2025, 7, 21, 13, 8, 45, 578, DateTimeKind.Utc).AddTicks(18) });

            migrationBuilder.CreateIndex(
                name: "IX_GameRatings_UserId_GameId",
                table: "GameRatings",
                columns: new[] { "UserId", "GameId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameRatings");

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 21, 7, 43, 40, 523, DateTimeKind.Utc).AddTicks(9450), new DateTime(2025, 7, 21, 7, 43, 40, 523, DateTimeKind.Utc).AddTicks(9450) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 21, 7, 43, 40, 523, DateTimeKind.Utc).AddTicks(9450), new DateTime(2025, 7, 21, 7, 43, 40, 523, DateTimeKind.Utc).AddTicks(9450) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 21, 7, 43, 40, 523, DateTimeKind.Utc).AddTicks(9450), new DateTime(2025, 7, 21, 7, 43, 40, 523, DateTimeKind.Utc).AddTicks(9450) });
        }
    }
}
