using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VC.Tenants.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tenants");

            migrationBuilder.CreateTable(
                name: "Tenants",
                schema: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Config_About = table.Column<string>(type: "text", nullable: false),
                    Config_Currency = table.Column<string>(type: "text", nullable: false),
                    Config_Language = table.Column<string>(type: "text", nullable: false),
                    Config_TimeZoneId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ContactInfo_Email = table.Column<string>(type: "text", nullable: true),
                    ContactInfo_Phone = table.Column<string>(type: "text", nullable: true),
                    ContactInfo_Address_Country = table.Column<string>(type: "text", nullable: false),
                    ContactInfo_Address_City = table.Column<string>(type: "text", nullable: false),
                    ContactInfo_Address_Street = table.Column<string>(type: "text", nullable: false),
                    ContactInfo_Address_House = table.Column<int>(type: "integer", nullable: false),
                    ContactInfo_IsVerify = table.Column<bool>(type: "boolean", nullable: false),
                    ContactInfo_ConfirmationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantDaySchedule",
                schema: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id1 = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    StartWork = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndWork = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantDaySchedule", x => new { x.Id, x.Id1 });
                    table.ForeignKey(
                        name: "FK_TenantDaySchedule_Tenants_Id",
                        column: x => x.Id,
                        principalSchema: "tenants",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantDaySchedule",
                schema: "tenants");

            migrationBuilder.DropTable(
                name: "Tenants",
                schema: "tenants");
        }
    }
}
