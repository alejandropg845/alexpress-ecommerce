using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations.Cart
{
    /// <inheritdoc />
    public partial class modifiedCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is50Discount",
                table: "CartProducts");

            migrationBuilder.DropColumn(
                name: "Is50OffOneProduct",
                table: "CartProducts");

            migrationBuilder.DropColumn(
                name: "IsShippingFree",
                table: "CartProducts");

            migrationBuilder.AddColumn<string>(
                name: "CouponName",
                table: "CartProducts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponName",
                table: "CartProducts");

            migrationBuilder.AddColumn<bool>(
                name: "Is50Discount",
                table: "CartProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Is50OffOneProduct",
                table: "CartProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShippingFree",
                table: "CartProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
