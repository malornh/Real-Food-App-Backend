using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RF1.Migrations
{
    /// <inheritdoc />
    public partial class Restrict_PhotoLink_Deletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farms_PhotoLinks_PhotoId",
                table: "Farms");

            migrationBuilder.DropForeignKey(
                name: "FK_Shops_PhotoLinks_PhotoId",
                table: "Shops");

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_PhotoLinks_PhotoId",
                table: "Farms",
                column: "PhotoId",
                principalTable: "PhotoLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_PhotoLinks_PhotoId",
                table: "Shops",
                column: "PhotoId",
                principalTable: "PhotoLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farms_PhotoLinks_PhotoId",
                table: "Farms");

            migrationBuilder.DropForeignKey(
                name: "FK_Shops_PhotoLinks_PhotoId",
                table: "Shops");

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_PhotoLinks_PhotoId",
                table: "Farms",
                column: "PhotoId",
                principalTable: "PhotoLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_PhotoLinks_PhotoId",
                table: "Shops",
                column: "PhotoId",
                principalTable: "PhotoLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
