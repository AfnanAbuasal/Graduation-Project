using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sconce.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedProgramEnrollmentsTableProficiencyFieldsToProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EvaluatorInstructorId",
                table: "Programs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExamWriterInstructorId",
                table: "Programs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasProficiencyExam",
                table: "Programs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProficiencyExamId",
                table: "Programs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExamId1",
                table: "ExamAttempts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentId1",
                table: "ExamAttempts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExamStatus",
                table: "Contents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 1);

            migrationBuilder.CreateTable(
                name: "ProgramEnrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProficiencyExamAttemptId = table.Column<int>(type: "int", nullable: true),
                    RecommendedCourseId = table.Column<int>(type: "int", nullable: true),
                    EvaluatedByInstructorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PlacedSectionId = table.Column<int>(type: "int", nullable: true),
                    EvaluatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramEnrollments_Courses_RecommendedCourseId",
                        column: x => x.RecommendedCourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramEnrollments_ExamAttempts_ProficiencyExamAttemptId",
                        column: x => x.ProficiencyExamAttemptId,
                        principalTable: "ExamAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramEnrollments_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramEnrollments_Sections_PlacedSectionId",
                        column: x => x.PlacedSectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramEnrollments_Users_EvaluatedByInstructorId",
                        column: x => x.EvaluatedByInstructorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramEnrollments_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Programs_EvaluatorInstructorId",
                table: "Programs",
                column: "EvaluatorInstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_ExamWriterInstructorId",
                table: "Programs",
                column: "ExamWriterInstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_ProficiencyExamId",
                table: "Programs",
                column: "ProficiencyExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttempts_ExamId1",
                table: "ExamAttempts",
                column: "ExamId1");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttempts_StudentId1",
                table: "ExamAttempts",
                column: "StudentId1");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramEnrollments_EvaluatedByInstructorId",
                table: "ProgramEnrollments",
                column: "EvaluatedByInstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramEnrollments_PlacedSectionId",
                table: "ProgramEnrollments",
                column: "PlacedSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramEnrollments_ProficiencyExamAttemptId",
                table: "ProgramEnrollments",
                column: "ProficiencyExamAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramEnrollments_ProgramId_StudentId",
                table: "ProgramEnrollments",
                columns: new[] { "ProgramId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramEnrollments_RecommendedCourseId",
                table: "ProgramEnrollments",
                column: "RecommendedCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramEnrollments_StudentId",
                table: "ProgramEnrollments",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamAttempts_Contents_ExamId1",
                table: "ExamAttempts",
                column: "ExamId1",
                principalTable: "Contents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamAttempts_Users_StudentId1",
                table: "ExamAttempts",
                column: "StudentId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_Contents_ProficiencyExamId",
                table: "Programs",
                column: "ProficiencyExamId",
                principalTable: "Contents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_Users_EvaluatorInstructorId",
                table: "Programs",
                column: "EvaluatorInstructorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Programs_Users_ExamWriterInstructorId",
                table: "Programs",
                column: "ExamWriterInstructorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamAttempts_Contents_ExamId1",
                table: "ExamAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamAttempts_Users_StudentId1",
                table: "ExamAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Contents_ProficiencyExamId",
                table: "Programs");

            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Users_EvaluatorInstructorId",
                table: "Programs");

            migrationBuilder.DropForeignKey(
                name: "FK_Programs_Users_ExamWriterInstructorId",
                table: "Programs");

            migrationBuilder.DropTable(
                name: "ProgramEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_Programs_EvaluatorInstructorId",
                table: "Programs");

            migrationBuilder.DropIndex(
                name: "IX_Programs_ExamWriterInstructorId",
                table: "Programs");

            migrationBuilder.DropIndex(
                name: "IX_Programs_ProficiencyExamId",
                table: "Programs");

            migrationBuilder.DropIndex(
                name: "IX_ExamAttempts_ExamId1",
                table: "ExamAttempts");

            migrationBuilder.DropIndex(
                name: "IX_ExamAttempts_StudentId1",
                table: "ExamAttempts");

            migrationBuilder.DropColumn(
                name: "EvaluatorInstructorId",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "ExamWriterInstructorId",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "HasProficiencyExam",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "ProficiencyExamId",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "ExamId1",
                table: "ExamAttempts");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "ExamAttempts");

            migrationBuilder.AlterColumn<int>(
                name: "ExamStatus",
                table: "Contents",
                type: "int",
                nullable: true,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
