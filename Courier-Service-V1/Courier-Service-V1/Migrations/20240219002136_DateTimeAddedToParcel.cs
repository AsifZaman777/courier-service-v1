using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courier_Service_V1.Migrations
{
    /// <inheritdoc />
    public partial class DateTimeAddedToParcel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancelDate",
                table: "Parcels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "Parcels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DispatchDate",
                table: "Parcels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PickupRequestDate",
                table: "Parcels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDate",
                table: "Parcels",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelDate",
                table: "Parcels");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "Parcels");

            migrationBuilder.DropColumn(
                name: "DispatchDate",
                table: "Parcels");

            migrationBuilder.DropColumn(
                name: "PickupRequestDate",
                table: "Parcels");

            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "Parcels");
        }
    }
}
