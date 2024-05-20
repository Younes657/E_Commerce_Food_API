using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce_Food_API.Migrations
{
    /// <inheritdoc />
    public partial class updateorderheaderproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentID",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripePaymentIntentID",
                table: "OrderHeaders");
        }
    }
}
