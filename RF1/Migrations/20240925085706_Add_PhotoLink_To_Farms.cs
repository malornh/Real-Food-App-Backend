using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RF1.Migrations
{
    /// <inheritdoc />
    public partial class Add_PhotoLink_To_Farms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farms_AspNetUsers_UserId",
                table: "Farms");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Farms");

            migrationBuilder.AddColumn<string>(
                name: "PhotoId",
                table: "Farms",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Farms_PhotoId",
                table: "Farms",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_AspNetUsers_UserId",
                table: "Farms",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_PhotoLinks_PhotoId",
                table: "Farms",
                column: "PhotoId",
                principalTable: "PhotoLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farms_AspNetUsers_UserId",
                table: "Farms");

            migrationBuilder.DropForeignKey(
                name: "FK_Farms_PhotoLinks_PhotoId",
                table: "Farms");

            migrationBuilder.DropIndex(
                name: "IX_Farms_PhotoId",
                table: "Farms");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Farms");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Farms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_AspNetUsers_UserId",
                table: "Farms",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
