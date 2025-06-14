using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GameStore.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "AuthorUserId", "CreatedAt", "Description", "GameUrl", "ThumbnailUrl", "Title" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2025, 6, 14, 13, 0, 8, 204, DateTimeKind.Utc).AddTicks(5961), "Guess the number between 1 and 100", "https://viplavmankar.github.io/Number-Guesser/", "https://github.com/ViplavMankar/Number-Guesser/blob/main/Images/Number%20Guesser.png?raw=true", "Number Guesser" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2025, 6, 14, 13, 0, 8, 204, DateTimeKind.Utc).AddTicks(5961), "Calculate your Body Mass Index (BMI)", "https://viplavmankar.github.io/BMI-Calculator/", "https://github.com/ViplavMankar/BMI-Calculator/blob/main/Screenshot%20from%202025-06-13%2013-07-58.png?raw=true", "BMI Calculator" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2025, 6, 14, 13, 0, 8, 204, DateTimeKind.Utc).AddTicks(5961), "Play the classic Pong game", "https://viplavmankar.github.io/Pong_Game/", "https://github.com/ViplavMankar/Pong_Game/blob/main/Screenshot%20from%202025-06-14%2011-45-30.png?raw=true", "Pong" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));
        }
    }
}
