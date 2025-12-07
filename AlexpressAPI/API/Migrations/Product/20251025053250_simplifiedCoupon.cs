using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations.Product
{
    /// <inheritdoc />
    public partial class simplifiedCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is50Discount",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "Is50OffOneProduct",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "IsShippingFree",
                table: "Coupons");

            migrationBuilder.AddColumn<string>(
                name: "CouponName",
                table: "Coupons",
                type: "nvarchar(20)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponName",
                table: "Coupons");

            migrationBuilder.AddColumn<bool>(
                name: "Is50Discount",
                table: "Coupons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Is50OffOneProduct",
                table: "Coupons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShippingFree",
                table: "Coupons",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
