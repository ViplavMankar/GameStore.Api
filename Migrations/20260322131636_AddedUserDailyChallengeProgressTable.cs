using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserDailyChallengeProgressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDailyChallengeProgress",
                table: "UserDailyChallengeProgress");

            migrationBuilder.RenameTable(
                name: "UserDailyChallengeProgress",
                newName: "UserDailyChallengeProgresses");

            migrationBuilder.RenameIndex(
                name: "IX_UserDailyChallengeProgress_UserId_ChallengeId",
                table: "UserDailyChallengeProgresses",
                newName: "IX_UserDailyChallengeProgresses_UserId_ChallengeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDailyChallengeProgresses",
                table: "UserDailyChallengeProgresses",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 22, 13, 16, 34, 847, DateTimeKind.Utc).AddTicks(6405), new DateTime(2026, 3, 22, 13, 16, 34, 847, DateTimeKind.Utc).AddTicks(6405) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 22, 13, 16, 34, 847, DateTimeKind.Utc).AddTicks(6405), new DateTime(2026, 3, 22, 13, 16, 34, 847, DateTimeKind.Utc).AddTicks(6405) });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 22, 13, 16, 34, 847, DateTimeKind.Utc).AddTicks(6405), new DateTime(2026, 3, 22, 13, 16, 34, 847, DateTimeKind.Utc).AddTicks(6405) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDailyChallengeProgresses",
                table: "UserDailyChallengeProgresses");

            migrationBuilder.RenameTable(
                name: "UserDailyChallengeProgresses",
                newName: "UserDailyChallengeProgress");

            migrationBuilder.RenameIndex(
                name: "IX_UserDailyChallengeProgresses_UserId_ChallengeId",
                table: "UserDailyChallengeProgress",
                newName: "IX_UserDailyChallengeProgress_UserId_ChallengeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDailyChallengeProgress",
                table: "UserDailyChallengeProgress",
                column: "Id");

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
        }
    }
}
