using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace I_Car_Services.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerIsApprovedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Customer_IsApproved",
                table: "Users",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customer_IsApproved",
                table: "Users");
        }
    }
}
