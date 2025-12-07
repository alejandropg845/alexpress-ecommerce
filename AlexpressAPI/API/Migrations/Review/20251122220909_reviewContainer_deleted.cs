using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations.Review
{
    /// <inheritdoc />
    public partial class reviewContainer_deleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewItems_ReviewContainer_ReviewContainerId",
                table: "ReviewItems");

            migrationBuilder.DropTable(
                name: "ReviewContainer");

            migrationBuilder.DropIndex(
                name: "IX_ReviewItems_ReviewContainerId",
                table: "ReviewItems");

            migrationBuilder.DropColumn(
                name: "ReviewContainerId",
                table: "ReviewItems");

            migrationBuilder.AlterColumn<byte>(
                name: "Rating",
                table: "ReviewItems",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "ReviewItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddColumn<int>(
                name: "ReviewContainerId",
                table: "ReviewItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReviewContainer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewContainer", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItems_ReviewContainerId",
                table: "ReviewItems",
                column: "ReviewContainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewItems_ReviewContainer_ReviewContainerId",
                table: "ReviewItems",
                column: "ReviewContainerId",
                principalTable: "ReviewContainer",
                principalColumn: "Id");
        }
    }
}
