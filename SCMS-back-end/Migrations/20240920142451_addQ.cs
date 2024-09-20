using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCMS_back_end.Migrations
{
    /// <inheritdoc />
    public partial class addQ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_Students_StudentId",
                table: "QuizResults");

            migrationBuilder.DropTable(
                name: "StudentAnswerResults");

            migrationBuilder.RenameColumn(
                name: "TotalQuestions",
                table: "QuizResults",
                newName: "NumbersOfCorrectAnswers");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Quizzes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Mark",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "Quizzes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StudentId1",
                table: "QuizResults",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_StudentId1",
                table: "QuizResults",
                column: "StudentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_Students_StudentId",
                table: "QuizResults",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_Students_StudentId1",
                table: "QuizResults",
                column: "StudentId1",
                principalTable: "Students",
                principalColumn: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_Students_StudentId",
                table: "QuizResults");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_Students_StudentId1",
                table: "QuizResults");

            migrationBuilder.DropIndex(
                name: "IX_QuizResults_StudentId1",
                table: "QuizResults");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Mark",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "QuizResults");

            migrationBuilder.RenameColumn(
                name: "NumbersOfCorrectAnswers",
                table: "QuizResults",
                newName: "TotalQuestions");

            migrationBuilder.CreateTable(
                name: "StudentAnswerResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizResultId = table.Column<int>(type: "int", nullable: false),
                    StudentAnswerId = table.Column<int>(type: "int", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswerResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAnswerResults_QuizResults_QuizResultId",
                        column: x => x.QuizResultId,
                        principalTable: "QuizResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAnswerResults_StudentAnswers_StudentAnswerId",
                        column: x => x.StudentAnswerId,
                        principalTable: "StudentAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswerResults_QuizResultId",
                table: "StudentAnswerResults",
                column: "QuizResultId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswerResults_StudentAnswerId",
                table: "StudentAnswerResults",
                column: "StudentAnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_Students_StudentId",
                table: "QuizResults",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
