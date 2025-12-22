using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sconce.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedFieldsToProgramCourseSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstructorId",
                table: "Sections",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Programs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PlannedCourseCount",
                table: "Programs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Programs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Courses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Courses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Sections_InstructorId",
                table: "Sections",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Users_InstructorId",
                table: "Sections",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Users_InstructorId",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Sections_InstructorId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "PlannedCourseCount",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Courses");
        }
    }
}
