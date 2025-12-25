using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sconce.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuestionFilePath",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AttemptsAllowed",
                table: "Contents",
                type: "int",
                nullable: true,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableFrom",
                table: "Contents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableTo",
                table: "Contents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Contents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExamStatus",
                table: "Contents",
                type: "int",
                nullable: true,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Exam_Title",
                table: "Contents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShuffleQuestions",
                table: "Contents",
                type: "bit",
                nullable: true,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionFilePath",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AttemptsAllowed",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "AvailableFrom",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "AvailableTo",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "ExamStatus",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "Exam_Title",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "ShuffleQuestions",
                table: "Contents");
        }
    }
}
