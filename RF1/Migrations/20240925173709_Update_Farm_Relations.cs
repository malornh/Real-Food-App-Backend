using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RF1.Migrations
{
    /// <inheritdoc />
    public partial class Update_Farm_Relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farms_PhotoLinks_PhotoId",
                table: "Farms");

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_PhotoLinks_PhotoId",
                table: "Farms",
                column: "PhotoId",
                principalTable: "PhotoLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farms_PhotoLinks_PhotoId",
                table: "Farms");

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_PhotoLinks_PhotoId",
                table: "Farms",
                column: "PhotoId",
                principalTable: "PhotoLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
