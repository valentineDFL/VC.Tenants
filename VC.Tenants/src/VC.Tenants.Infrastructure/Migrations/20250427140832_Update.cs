using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VC.Tenants.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactInfo_IsVerify",
                schema: "tenants",
                table: "Tenants",
                newName: "ContactInfo_EmailAddress_IsVerify");

            migrationBuilder.RenameColumn(
                name: "ContactInfo_Email",
                schema: "tenants",
                table: "Tenants",
                newName: "ContactInfo_EmailAddress_Email");

            migrationBuilder.RenameColumn(
                name: "ContactInfo_ConfirmationTime",
                schema: "tenants",
                table: "Tenants",
                newName: "ContactInfo_EmailAddress_ConfirmationTime");

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo_EmailAddress_Code",
                schema: "tenants",
                table: "Tenants",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactInfo_EmailAddress_Code",
                schema: "tenants",
                table: "Tenants");

            migrationBuilder.RenameColumn(
                name: "ContactInfo_EmailAddress_IsVerify",
                schema: "tenants",
                table: "Tenants",
                newName: "ContactInfo_IsVerify");

            migrationBuilder.RenameColumn(
                name: "ContactInfo_EmailAddress_Email",
                schema: "tenants",
                table: "Tenants",
                newName: "ContactInfo_Email");

            migrationBuilder.RenameColumn(
                name: "ContactInfo_EmailAddress_ConfirmationTime",
                schema: "tenants",
                table: "Tenants",
                newName: "ContactInfo_ConfirmationTime");
        }
    }
}
