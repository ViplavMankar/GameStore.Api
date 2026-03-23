using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GameStore.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GamesPlayed = table.Column<int>(type: "integer", nullable: false),
                    SessionsCount = table.Column<int>(type: "integer", nullable: false),
                    TotalMinutesPlayed = table.Column<int>(type: "integer", nullable: false),
                    TotalPurchases = table.Column<int>(type: "integer", nullable: false),
                    LastPlayedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStatistics", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Achievements",
                columns: new[] { "Id", "ConditionType", "Description", "Icon", "Name", "TargetValue" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Sessions", "Play your first game", "/icons/first.png", "First Game", 1 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Sessions", "Play 5 sessions", "/icons/five.png", "Getting Started", 5 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "PlayTime", "Play for 60 minutes", "/icons/hour.png", "1 Hour Gamer", 60 }
                });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 18, 16, 12, 53, 223, DateTimeKind.Utc).AddTicks(2983), new DateTime(2026, 3, 18, 16, 12, 53, 223, DateTimeKind.Utc).AddTicks(2983) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 18, 16, 12, 53, 223, DateTimeKind.Utc).AddTicks(2983), new DateTime(2026, 3, 18, 16, 12, 53, 223, DateTimeKind.Utc).AddTicks(2983) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 18, 16, 12, 53, 223, DateTimeKind.Utc).AddTicks(2983), new DateTime(2026, 3, 18, 16, 12, 53, 223, DateTimeKind.Utc).AddTicks(2983) });

            migrationBuilder.CreateIndex(
                name: "IX_UserStatistics_UserId",
                table: "UserStatistics",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserStatistics");

            migrationBuilder.DeleteData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 18, 15, 42, 58, 556, DateTimeKind.Utc).AddTicks(1059), new DateTime(2026, 3, 18, 15, 42, 58, 556, DateTimeKind.Utc).AddTicks(1059) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 18, 15, 42, 58, 556, DateTimeKind.Utc).AddTicks(1059), new DateTime(2026, 3, 18, 15, 42, 58, 556, DateTimeKind.Utc).AddTicks(1059) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 18, 15, 42, 58, 556, DateTimeKind.Utc).AddTicks(1059), new DateTime(2026, 3, 18, 15, 42, 58, 556, DateTimeKind.Utc).AddTicks(1059) });
        }
    }
}
