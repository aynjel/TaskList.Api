using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskList.Infrastucture.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStatusDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TaskItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Todo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TaskItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Todo",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
