using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedChallengesAndXPConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalXP",
                table: "UserStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DailyChallenges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TargetValue = table.Column<int>(type: "integer", nullable: false),
                    XPReward = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyChallenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDailyChallengeProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChallengeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentProgress = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDailyChallengeProgress", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 22, 9, 14, 44, 839, DateTimeKind.Utc).AddTicks(8943), new DateTime(2026, 3, 22, 9, 14, 44, 839, DateTimeKind.Utc).AddTicks(8943) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 22, 9, 14, 44, 839, DateTimeKind.Utc).AddTicks(8943), new DateTime(2026, 3, 22, 9, 14, 44, 839, DateTimeKind.Utc).AddTicks(8943) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 22, 9, 14, 44, 839, DateTimeKind.Utc).AddTicks(8943), new DateTime(2026, 3, 22, 9, 14, 44, 839, DateTimeKind.Utc).AddTicks(8943) });

            migrationBuilder.CreateIndex(
                name: "IX_UserDailyChallengeProgress_UserId_ChallengeId",
                table: "UserDailyChallengeProgress",
                columns: new[] { "UserId", "ChallengeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyChallenges");

            migrationBuilder.DropTable(
                name: "UserDailyChallengeProgress");

            migrationBuilder.DropColumn(
                name: "TotalXP",
                table: "UserStatistics");

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
    }
}
