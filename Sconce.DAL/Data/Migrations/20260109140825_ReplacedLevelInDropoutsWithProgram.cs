using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sconce.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplacedLevelInDropoutsWithProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dropouts_Levels_LevelId",
                table: "Dropouts");

            migrationBuilder.RenameColumn(
                name: "LevelId",
                table: "Dropouts",
                newName: "ProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_Dropouts_LevelId",
                table: "Dropouts",
                newName: "IX_Dropouts_ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dropouts_Programs_ProgramId",
                table: "Dropouts",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dropouts_Programs_ProgramId",
                table: "Dropouts");

            migrationBuilder.RenameColumn(
                name: "ProgramId",
                table: "Dropouts",
                newName: "LevelId");

            migrationBuilder.RenameIndex(
                name: "IX_Dropouts_ProgramId",
                table: "Dropouts",
                newName: "IX_Dropouts_LevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dropouts_Levels_LevelId",
                table: "Dropouts",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
