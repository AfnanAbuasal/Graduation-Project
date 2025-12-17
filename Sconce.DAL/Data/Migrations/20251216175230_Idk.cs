using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sconce.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class Idk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrerequisiteProgramId",
                table: "Programs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Programs_PrerequisiteProgramId",
                table: "Programs",
                column: "PrerequisiteProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_Programs_PrerequisiteProgramId",
                table: "Programs",
                column: "PrerequisiteProgramId",
                principalTable: "Programs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Programs_PrerequisiteProgramId",
                table: "Programs");

            migrationBuilder.DropIndex(
                name: "IX_Programs_PrerequisiteProgramId",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "PrerequisiteProgramId",
                table: "Programs");
        }
    }
}
