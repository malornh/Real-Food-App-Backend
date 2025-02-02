using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RF1.Migrations
{
    /// <inheritdoc />
    public partial class ChangePhotosArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farms_PhotoLinks_PhotoId",
                table: "Farms");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_PhotoLinks_PhotoId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Shops_PhotoLinks_PhotoId",
                table: "Shops");

            migrationBuilder.DropTable(
                name: "PhotoLinks");

            migrationBuilder.DropIndex(
                name: "IX_Shops_PhotoId",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Products_PhotoId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Farms_PhotoId",
                table: "Farms");

            migrationBuilder.RenameColumn(
                name: "PhotoId",
                table: "Shops",
                newName: "PhotoUrl");

            migrationBuilder.RenameColumn(
                name: "PhotoId",
                table: "Products",
                newName: "PhotoUrl");

            migrationBuilder.RenameColumn(
                name: "PhotoId",
                table: "Farms",
                newName: "PhotoUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Shops",
                newName: "PhotoId");

            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Products",
                newName: "PhotoId");

            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Farms",
                newName: "PhotoId");

            migrationBuilder.CreateTable(
                name: "PhotoLinks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoLinks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shops_PhotoId",
                table: "Shops",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_PhotoId",
                table: "Products",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Farms_PhotoId",
                table: "Farms",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoLinks_UserId",
                table: "PhotoLinks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_PhotoLinks_PhotoId",
                table: "Farms",
                column: "PhotoId",
                principalTable: "PhotoLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_PhotoLinks_PhotoId",
                table: "Products",
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
    }
}
