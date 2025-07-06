using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class lk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LinhKien",
                columns: table => new
                {
                    malinhkien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenlinhkien = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    maloailinhkien = table.Column<int>(type: "int", nullable: false),
                    hangsanxuat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    gianhap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    giaban = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    soluongton = table.Column<int>(type: "int", nullable: false),
                    thongsoktyhuat = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ngaytao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    trangthai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinhKien", x => x.malinhkien);
                    table.ForeignKey(
                        name: "FK_LinhKien_LoaiLinhKien_maloailinhkien",
                        column: x => x.maloailinhkien,
                        principalTable: "LoaiLinhKien",
                        principalColumn: "maloailinhkien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_giaban",
                table: "LinhKien",
                column: "giaban");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_hangsanxuat",
                table: "LinhKien",
                column: "hangsanxuat");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_maloailinhkien",
                table: "LinhKien",
                column: "maloailinhkien");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_ngaytao",
                table: "LinhKien",
                column: "ngaytao");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_soluongton",
                table: "LinhKien",
                column: "soluongton");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_tenlinhkien",
                table: "LinhKien",
                column: "tenlinhkien");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_trangthai",
                table: "LinhKien",
                column: "trangthai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinhKien");
        }
    }
}
