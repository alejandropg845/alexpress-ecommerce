using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations.Order
{
    /// <inheritdoc />
    public partial class reviewContainer_deleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewContainerId",
                table: "Orders");

            migrationBuilder.AddColumn<byte>(
                name: "Rating",
                table: "Orders",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "ReviewContainerId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
