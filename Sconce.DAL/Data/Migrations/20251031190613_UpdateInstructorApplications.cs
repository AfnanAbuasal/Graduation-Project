using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sconce.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInstructorApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationStatus",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CVPath",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Users",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Users",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ExperienceWithTeachingKids",
                table: "Users",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LevelOfProficiency",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Parent_DateOfBirth",
                table: "Users",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Parent_Gender",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Student_ApplicationStatus",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Student_DateOfBirth",
                table: "Users",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Student_Gender",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Student_SubmittedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InstructorApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    ExperienceWithTeachingKids = table.Column<bool>(type: "bit", nullable: false),
                    CVPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationStatus = table.Column<int>(type: "int", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParentInvites",
                columns: table => new
                {
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GuardianEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentInvites", x => x.Token);
                    table.ForeignKey(
                        name: "FK_ParentInvites_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentParents",
                columns: table => new
                {
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RelationshipWithStudent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentParents", x => new { x.StudentId, x.ParentId });
                    table.ForeignKey(
                        name: "FK_StudentParents_Users_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentParents_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParentInvites_StudentId",
                table: "ParentInvites",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentParents_ParentId",
                table: "StudentParents",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstructorApplications");

            migrationBuilder.DropTable(
                name: "ParentInvites");

            migrationBuilder.DropTable(
                name: "StudentParents");

            migrationBuilder.DropColumn(
                name: "ApplicationStatus",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CVPath",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DocumentPath",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ExperienceWithTeachingKids",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LevelOfProficiency",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Parent_DateOfBirth",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Parent_Gender",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Student_ApplicationStatus",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Student_DateOfBirth",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Student_Gender",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Student_SubmittedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "Users");
        }
    }
}
