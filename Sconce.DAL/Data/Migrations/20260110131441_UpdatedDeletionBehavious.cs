using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sconce.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDeletionBehavious : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Levels_Levels_PrerequisiteLevelId",
                table: "Levels");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramEnrollments_Programs_ProgramId",
                table: "ProgramEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Courses_CourseId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Programs_ProgramId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Users_InstructorId",
                table: "Sections");

            migrationBuilder.AddForeignKey(
                name: "FK_Levels_Levels_PrerequisiteLevelId",
                table: "Levels",
                column: "PrerequisiteLevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramEnrollments_Programs_ProgramId",
                table: "ProgramEnrollments",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Courses_CourseId",
                table: "Questions",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Programs_ProgramId",
                table: "Questions",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Users_InstructorId",
                table: "Sections",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Levels_Levels_PrerequisiteLevelId",
                table: "Levels");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramEnrollments_Programs_ProgramId",
                table: "ProgramEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Courses_CourseId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Programs_ProgramId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Users_InstructorId",
                table: "Sections");

            migrationBuilder.AddForeignKey(
                name: "FK_Levels_Levels_PrerequisiteLevelId",
                table: "Levels",
                column: "PrerequisiteLevelId",
                principalTable: "Levels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramEnrollments_Programs_ProgramId",
                table: "ProgramEnrollments",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Courses_CourseId",
                table: "Questions",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Programs_ProgramId",
                table: "Questions",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Users_InstructorId",
                table: "Sections",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
