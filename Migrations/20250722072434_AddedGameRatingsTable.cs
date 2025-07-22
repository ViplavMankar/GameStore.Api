using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GameStore.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedGameRatingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: false),
                    GameUrl = table.Column<string>(type: "text", nullable: false),
                    AuthorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

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
                        name: "FK_GameRatings_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCollections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCollections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCollections_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "AuthorUserId", "CreatedAt", "Description", "GameUrl", "ThumbnailUrl", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2025, 7, 22, 7, 24, 33, 173, DateTimeKind.Utc).AddTicks(6504), "Guess the number between 1 and 100", "https://viplavmankar.github.io/Number-Guesser/", "https://github.com/ViplavMankar/Number-Guesser/blob/main/Images/Number%20Guesser.png?raw=true", "Number Guesser", new DateTime(2025, 7, 22, 7, 24, 33, 173, DateTimeKind.Utc).AddTicks(6504) },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2025, 7, 22, 7, 24, 33, 173, DateTimeKind.Utc).AddTicks(6504), "Calculate your Body Mass Index (BMI)", "https://viplavmankar.github.io/BMI-Calculator/", "https://github.com/ViplavMankar/BMI-Calculator/blob/main/Screenshot%20from%202025-06-13%2013-07-58.png?raw=true", "BMI Calculator", new DateTime(2025, 7, 22, 7, 24, 33, 173, DateTimeKind.Utc).AddTicks(6504) },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2025, 7, 22, 7, 24, 33, 173, DateTimeKind.Utc).AddTicks(6504), "Play the classic Pong game", "https://viplavmankar.github.io/Pong_Game/", "https://github.com/ViplavMankar/Pong_Game/blob/main/Screenshot%20from%202025-06-14%2011-45-30.png?raw=true", "Pong", new DateTime(2025, 7, 22, 7, 24, 33, 173, DateTimeKind.Utc).AddTicks(6504) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameRatings_GameId",
                table: "GameRatings",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameRatings_UserId_GameId",
                table: "GameRatings",
                columns: new[] { "UserId", "GameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCollections_GameId",
                table: "UserCollections",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCollections_UserId_GameId",
                table: "UserCollections",
                columns: new[] { "UserId", "GameId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameRatings");

            migrationBuilder.DropTable(
                name: "UserCollections");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
