using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedLeaderboardStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalMinutesPlayed",
                table: "UserStatistics",
                newName: "TotalSecondsPlayed");

            migrationBuilder.CreateTable(
                name: "LeaderboardEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    TotalPlayTimeSeconds = table.Column<int>(type: "integer", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardEntries", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 21, 19, 25, 43, 332, DateTimeKind.Utc).AddTicks(6391), new DateTime(2026, 3, 21, 19, 25, 43, 332, DateTimeKind.Utc).AddTicks(6391) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 21, 19, 25, 43, 332, DateTimeKind.Utc).AddTicks(6391), new DateTime(2026, 3, 21, 19, 25, 43, 332, DateTimeKind.Utc).AddTicks(6391) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 21, 19, 25, 43, 332, DateTimeKind.Utc).AddTicks(6391), new DateTime(2026, 3, 21, 19, 25, 43, 332, DateTimeKind.Utc).AddTicks(6391) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaderboardEntries");

            migrationBuilder.RenameColumn(
                name: "TotalSecondsPlayed",
                table: "UserStatistics",
                newName: "TotalMinutesPlayed");

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
        }
    }
}
