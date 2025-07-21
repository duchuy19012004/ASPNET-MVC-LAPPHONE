using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class dichvu1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DichVuLinhKien",
                columns: table => new
                {
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    MaLinhKien = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVuLinhKien", x => new { x.MaDichVu, x.MaLinhKien });
                    table.ForeignKey(
                        name: "FK_DichVuLinhKien_DichVu_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVu",
                        principalColumn: "madichvu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DichVuLinhKien_LinhKien_MaLinhKien",
                        column: x => x.MaLinhKien,
                        principalTable: "LinhKien",
                        principalColumn: "malinhkien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DichVuLinhKien_MaLinhKien",
                table: "DichVuLinhKien",
                column: "MaLinhKien");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DichVuLinhKien");
        }
    }
}
