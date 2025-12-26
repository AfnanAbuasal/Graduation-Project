using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sconce.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRelationshipBetweenChoiceMCQ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choices_Questions_MultipleChoiceQuestionId",
                table: "Choices");

            migrationBuilder.DropIndex(
                name: "IX_Choices_MultipleChoiceQuestionId",
                table: "Choices");

            migrationBuilder.DropColumn(
                name: "MultipleChoiceQuestionId",
                table: "Choices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MultipleChoiceQuestionId",
                table: "Choices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Choices_MultipleChoiceQuestionId",
                table: "Choices",
                column: "MultipleChoiceQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Choices_Questions_MultipleChoiceQuestionId",
                table: "Choices",
                column: "MultipleChoiceQuestionId",
                principalTable: "Questions",
                principalColumn: "Id");
        }
    }
}
