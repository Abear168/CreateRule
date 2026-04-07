using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreateRule.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInnerBoxRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InnerBoxRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkOrder = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Template = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Prefix = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PrefixLength = table.Column<int>(type: "INTEGER", nullable: false),
                    Constant = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SequenceLength = table.Column<int>(type: "INTEGER", nullable: false),
                    SequenceStart = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedById = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InnerBoxRules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InnerBoxRules_WorkOrder",
                table: "InnerBoxRules",
                column: "WorkOrder",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InnerBoxRules");
        }
    }
}
