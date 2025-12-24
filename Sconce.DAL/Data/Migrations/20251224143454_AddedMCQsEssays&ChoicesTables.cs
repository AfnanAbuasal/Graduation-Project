using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sconce.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedMCQsEssaysChoicesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowFileUpload",
                table: "Questions",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowMultipleSelections",
                table: "Questions",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Questions",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaxFileSizeMb",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxWords",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShuffleChoices",
                table: "Questions",
                type: "bit",
                nullable: true,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "Choices",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    MultipleChoiceQuestionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Choices", x => new { x.QuestionId, x.Text });
                    table.ForeignKey(
                        name: "FK_Choices_Questions_MultipleChoiceQuestionId",
                        column: x => x.MultipleChoiceQuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Choices_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Choices_MultipleChoiceQuestionId",
                table: "Choices",
                column: "MultipleChoiceQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Choices_SortOrder",
                table: "Choices",
                column: "SortOrder",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Choices");

            migrationBuilder.DropColumn(
                name: "AllowFileUpload",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AllowMultipleSelections",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "MaxFileSizeMb",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "MaxWords",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ShuffleChoices",
                table: "Questions");
        }
    }
}
