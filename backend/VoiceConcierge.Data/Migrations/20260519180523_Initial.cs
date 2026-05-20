using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace VoiceConcierge.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "faq_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Question = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Answer = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Embedding = table.Column<Vector>(type: "vector(1536)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_faq_items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "unanswered_questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Question = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Embedding = table.Column<Vector>(type: "vector(1536)", nullable: false),
                    Frequency = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FirstAskedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastAskedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConvertedToFaqId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unanswered_questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "voice_config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ActiveVoiceId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voice_config", x => x.Id);
                    table.CheckConstraint("ck_voice_config_singleton", "\"Id\" = 1");
                });

            migrationBuilder.CreateIndex(
                name: "IX_faq_items_Category",
                table: "faq_items",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_faq_items_Embedding",
                table: "faq_items",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_unanswered_questions_Embedding",
                table: "unanswered_questions",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_unanswered_questions_LastAskedAt",
                table: "unanswered_questions",
                column: "LastAskedAt");

            migrationBuilder.CreateIndex(
                name: "IX_unanswered_questions_Status",
                table: "unanswered_questions",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "faq_items");

            migrationBuilder.DropTable(
                name: "unanswered_questions");

            migrationBuilder.DropTable(
                name: "voice_config");
        }
    }
}
