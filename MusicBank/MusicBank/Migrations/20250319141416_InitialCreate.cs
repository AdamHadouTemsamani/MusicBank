using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MusicBank.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Delete the existing tables
            migrationBuilder.Sql("DROP TABLE IF EXISTS ticket_reservation");
            migrationBuilder.Sql("DROP TABLE IF EXISTS event");
            migrationBuilder.Sql("DROP TABLE IF EXISTS user");

            //Create the new tables
            migrationBuilder.CreateTable(
                name: "event",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_name = table.Column<string>(type: "TEXT", nullable: false),
                    event_venue = table.Column<string>(type: "TEXT", nullable: false),
                    event_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event", x => x.event_id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    phone_number = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "ticket_reservation",
                columns: table => new
                {
                    ticket_reservation_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    reservation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket_reservation", x => x.ticket_reservation_id);
                    table.ForeignKey(
                        name: "FK_ticket_reservation_event_EventId",
                        column: x => x.EventId,
                        principalTable: "event",
                        principalColumn: "event_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ticket_reservation_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ticket_reservation_EventId",
                table: "ticket_reservation",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_reservation_UserId",
                table: "ticket_reservation",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ticket_reservation");

            migrationBuilder.DropTable(
                name: "event");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
