using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sconce.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedGradingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExamId1",
                table: "ExamQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GradedAt",
                table: "ExamAttempts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExamAttemptId1",
                table: "Answers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GradedAt",
                table: "Answers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GradedByInstructorId",
                table: "Answers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxScore",
                table: "Answers",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Score",
                table: "Answers",
                type: "decimal(6,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestions_ExamId1",
                table: "ExamQuestions",
                column: "ExamId1");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_ExamAttemptId1",
                table: "Answers",
                column: "ExamAttemptId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_ExamAttempts_ExamAttemptId1",
                table: "Answers",
                column: "ExamAttemptId1",
                principalTable: "ExamAttempts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamQuestions_Contents_ExamId1",
                table: "ExamQuestions",
                column: "ExamId1",
                principalTable: "Contents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_ExamAttempts_ExamAttemptId1",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamQuestions_Contents_ExamId1",
                table: "ExamQuestions");

            migrationBuilder.DropIndex(
                name: "IX_ExamQuestions_ExamId1",
                table: "ExamQuestions");

            migrationBuilder.DropIndex(
                name: "IX_Answers_ExamAttemptId1",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "ExamId1",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "GradedAt",
                table: "ExamAttempts");

            migrationBuilder.DropColumn(
                name: "ExamAttemptId1",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "GradedAt",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "GradedByInstructorId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Answers");
        }
    }
}
