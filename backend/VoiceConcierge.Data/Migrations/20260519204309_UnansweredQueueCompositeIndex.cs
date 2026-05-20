using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoiceConcierge.Data.Migrations
{
    /// <inheritdoc />
    public partial class UnansweredQueueCompositeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_unanswered_questions_LastAskedAt",
                table: "unanswered_questions");

            migrationBuilder.DropIndex(
                name: "IX_unanswered_questions_Status",
                table: "unanswered_questions");

            migrationBuilder.CreateIndex(
                name: "IX_unanswered_questions_Status_Frequency_LastAskedAt",
                table: "unanswered_questions",
                columns: new[] { "Status", "Frequency", "LastAskedAt" },
                descending: new[] { false, true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_unanswered_questions_Status_Frequency_LastAskedAt",
                table: "unanswered_questions");

            migrationBuilder.CreateIndex(
                name: "IX_unanswered_questions_LastAskedAt",
                table: "unanswered_questions",
                column: "LastAskedAt");

            migrationBuilder.CreateIndex(
                name: "IX_unanswered_questions_Status",
                table: "unanswered_questions",
                column: "Status");
        }
    }
}
