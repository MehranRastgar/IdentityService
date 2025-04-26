using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityService.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPhysicalInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityLevel",
                schema: "identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Age",
                schema: "identity",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                schema: "identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Goal",
                schema: "identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                schema: "identity",
                table: "AspNetUsers",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                schema: "identity",
                table: "AspNetUsers",
                type: "decimal(5,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityLevel",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Age",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Goal",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Height",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Weight",
                schema: "identity",
                table: "AspNetUsers");
        }
    }
}
