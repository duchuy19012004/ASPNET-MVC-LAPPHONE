using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace phonev2.Migrations
{
    /// <inheritdoc />
    public partial class nhacungcap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    manhacungcap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tennhacungcap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    diachi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    sodienthoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ngaytao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    trangthai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaCungCap", x => x.manhacungcap);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NhaCungCap_email",
                table: "NhaCungCap",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhaCungCap_sodienthoai",
                table: "NhaCungCap",
                column: "sodienthoai");

            migrationBuilder.CreateIndex(
                name: "IX_NhaCungCap_tennhacungcap",
                table: "NhaCungCap",
                column: "tennhacungcap");

            migrationBuilder.CreateIndex(
                name: "IX_NhaCungCap_trangthai",
                table: "NhaCungCap",
                column: "trangthai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NhaCungCap");
        }
    }
}
