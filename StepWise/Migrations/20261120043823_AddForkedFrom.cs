using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StepWise.Migrations
{
    /// <inheritdoc />
    public partial class AddForkedFrom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ForkedFromId",
                table: "Instructions",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForkedFromId",
                table: "Instructions");
        }
    }
}
