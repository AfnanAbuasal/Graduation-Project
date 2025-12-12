using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sconce.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Assignments_AssignmentId",
                table: "Submission");

            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Users_StudentId",
                table: "Submission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Submission",
                table: "Submission");

            migrationBuilder.RenameTable(
                name: "Submission",
                newName: "Submissions");

            migrationBuilder.RenameIndex(
                name: "IX_Submission_StudentId",
                table: "Submissions",
                newName: "IX_Submissions_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Submission_AssignmentId",
                table: "Submissions",
                newName: "IX_Submissions_AssignmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Submissions",
                table: "Submissions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Assignments_AssignmentId",
                table: "Submissions",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Users_StudentId",
                table: "Submissions",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Assignments_AssignmentId",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_StudentId",
                table: "Submissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Submissions",
                table: "Submissions");

            migrationBuilder.RenameTable(
                name: "Submissions",
                newName: "Submission");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_StudentId",
                table: "Submission",
                newName: "IX_Submission_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_AssignmentId",
                table: "Submission",
                newName: "IX_Submission_AssignmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Submission",
                table: "Submission",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_Assignments_AssignmentId",
                table: "Submission",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_Users_StudentId",
                table: "Submission",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
