using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCMS_back_end.Migrations
{
    /// <inheritdoc />
    public partial class addStudentAnswerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_Students_StudentId",
                table: "QuizResults");

            migrationBuilder.CreateTable(
                name: "StudentAnswerResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentAnswerId = table.Column<int>(type: "int", nullable: false),
                    QuizResultId = table.Column<int>(type: "int", nullable: false),
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_Students_StudentId",
                table: "QuizResults");

            migrationBuilder.DropTable(
                name: "StudentAnswerResults");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_Students_StudentId",
                table: "QuizResults",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
