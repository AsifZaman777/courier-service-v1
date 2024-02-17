using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courier_Service_V1.Migrations
{
    /// <inheritdoc />
    public partial class modifiedParcelTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parcels_Merchants_MerchantId",
                table: "Parcels");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Parcels");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProductWeight",
                table: "Parcels",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ProductPrice",
                table: "Parcels",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MerchantId",
                table: "Parcels",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "DeliveryCharge",
                table: "Parcels",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Parcels_Merchants_MerchantId",
                table: "Parcels",
                column: "MerchantId",
                principalTable: "Merchants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parcels_Merchants_MerchantId",
                table: "Parcels");

            migrationBuilder.AlterColumn<string>(
                name: "ProductWeight",
                table: "Parcels",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "ProductPrice",
                table: "Parcels",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MerchantId",
                table: "Parcels",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryCharge",
                table: "Parcels",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "TotalPrice",
                table: "Parcels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Parcels_Merchants_MerchantId",
                table: "Parcels",
                column: "MerchantId",
                principalTable: "Merchants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
