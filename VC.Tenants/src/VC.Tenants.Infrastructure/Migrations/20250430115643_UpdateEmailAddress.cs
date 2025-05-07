using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VC.Tenants.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmailAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactInfo_EmailAddress_Code",
                schema: "tenants",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "ContactInfo_EmailAddress_ConfirmationTime",
                schema: "tenants",
                table: "Tenants");

            migrationBuilder.RenameColumn(
                name: "ContactInfo_EmailAddress_IsVerify",
                schema: "tenants",
                table: "Tenants",
                newName: "ContactInfo_EmailAddress_IsConfirmed");

            migrationBuilder.AlterColumn<string>(
                name: "ContactInfo_Phone",
                schema: "tenants",
                table: "Tenants",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContactInfo_EmailAddress_Email",
                schema: "tenants",
                table: "Tenants",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactInfo_EmailAddress_IsConfirmed",
                schema: "tenants",
                table: "Tenants",
                newName: "ContactInfo_EmailAddress_IsVerify");

            migrationBuilder.AlterColumn<string>(
                name: "ContactInfo_Phone",
                schema: "tenants",
                table: "Tenants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ContactInfo_EmailAddress_Email",
                schema: "tenants",
                table: "Tenants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo_EmailAddress_Code",
                schema: "tenants",
                table: "Tenants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ContactInfo_EmailAddress_ConfirmationTime",
                schema: "tenants",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
