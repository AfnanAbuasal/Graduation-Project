using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sconce.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropOldChoicesCreateNewOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop old table (this deletes all Choice rows)
            migrationBuilder.DropTable(name: "Choices");

            // Recreate table with Id as Identity PK
            migrationBuilder.CreateTable(
                name: "Choices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    QuestionId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IsCorrect = table.Column<bool>(nullable: false, defaultValue: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Choices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Choices_MultipleChoiceQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Preserve old composite-key uniqueness
            migrationBuilder.CreateIndex(
                name: "IX_Choices_QuestionId_Text",
                table: "Choices",
                columns: new[] { "QuestionId", "Text" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Choices_QuestionId",
                table: "Choices",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Choices");
        }
    }
}
