using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class thietbi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThietBi",
                columns: table => new
                {
                    mathietbi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    makhachhang = table.Column<int>(type: "int", nullable: false),
                    tenthietbi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    loaithietbi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    hangsanxuat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    soserial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThietBi", x => x.mathietbi);
                    table.ForeignKey(
                        name: "FK_ThietBi_KhachHang_makhachhang",
                        column: x => x.makhachhang,
                        principalTable: "KhachHang",
                        principalColumn: "makhachhang",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThietBi_hangsanxuat",
                table: "ThietBi",
                column: "hangsanxuat");

            migrationBuilder.CreateIndex(
                name: "IX_ThietBi_loaithietbi",
                table: "ThietBi",
                column: "loaithietbi");

            migrationBuilder.CreateIndex(
                name: "IX_ThietBi_makhachhang",
                table: "ThietBi",
                column: "makhachhang");

            migrationBuilder.CreateIndex(
                name: "IX_ThietBi_soserial",
                table: "ThietBi",
                column: "soserial");

            migrationBuilder.CreateIndex(
                name: "IX_ThietBi_tenthietbi",
                table: "ThietBi",
                column: "tenthietbi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThietBi");
        }
    }
}
