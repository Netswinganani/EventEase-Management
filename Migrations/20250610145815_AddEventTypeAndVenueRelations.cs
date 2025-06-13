using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddEventTypeAndVenueRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    EventTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.EventTypeID);
                });

            migrationBuilder.CreateTable(
                name: "VenueManager",
                columns: table => new
                {
                    VenueID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueManager", x => x.VenueID);
                });

            migrationBuilder.CreateTable(
                name: "EventManager",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventTypeID = table.Column<int>(type: "int", maxLength: 30, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VenueID = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VenueID1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventManager", x => x.EventID);
                    table.ForeignKey(
                        name: "FK_EventManager_EventTypes_EventTypeID",
                        column: x => x.EventTypeID,
                        principalTable: "EventTypes",
                        principalColumn: "EventTypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventManager_VenueManager_VenueID",
                        column: x => x.VenueID,
                        principalTable: "VenueManager",
                        principalColumn: "VenueID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventManager_VenueManager_VenueID1",
                        column: x => x.VenueID1,
                        principalTable: "VenueManager",
                        principalColumn: "VenueID");
                });

            migrationBuilder.CreateTable(
                name: "BookingManager",
                columns: table => new
                {
                    BookingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EventBooking = table.Column<int>(type: "int", nullable: false),
                    VenueBooking = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventID = table.Column<int>(type: "int", nullable: true),
                    VenueId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingManager", x => x.BookingID);
                    table.ForeignKey(
                        name: "FK_BookingManager_EventManager_EventBooking",
                        column: x => x.EventBooking,
                        principalTable: "EventManager",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingManager_EventManager_VenueBooking",
                        column: x => x.VenueBooking,
                        principalTable: "EventManager",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingManager_VenueManager_VenueId",
                        column: x => x.VenueId,
                        principalTable: "VenueManager",
                        principalColumn: "VenueID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingManager_EventBooking",
                table: "BookingManager",
                column: "EventBooking");

            migrationBuilder.CreateIndex(
                name: "IX_BookingManager_VenueBooking",
                table: "BookingManager",
                column: "VenueBooking");

            migrationBuilder.CreateIndex(
                name: "IX_BookingManager_VenueId",
                table: "BookingManager",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_EventManager_EventTypeID",
                table: "EventManager",
                column: "EventTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EventManager_VenueID",
                table: "EventManager",
                column: "VenueID");

            migrationBuilder.CreateIndex(
                name: "IX_EventManager_VenueID1",
                table: "EventManager",
                column: "VenueID1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingManager");

            migrationBuilder.DropTable(
                name: "EventManager");

            migrationBuilder.DropTable(
                name: "EventTypes");

            migrationBuilder.DropTable(
                name: "VenueManager");
        }
    }
}
