using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.Migrations
{
    public partial class SeedRole : Migration
    {
        private string UserTechnicianId = Guid.NewGuid().ToString();
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SeedRolesSQL(migrationBuilder);
            SeedUserRoles(migrationBuilder);
        }
        private void SeedRolesSQL(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"INSERT INTO [dbo].[AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp])
            VALUES ('{UserTechnicianId}', 'Technician', 'TECHNICIAN', null);");
        }
        private void SeedUserRoles(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
        INSERT INTO [dbo].[AspNetUserRoles]
           ([UserId]
           ,[RoleId])
        VALUES
           ('f6085804-a077-4bca-8a76-fa0d1aba1697', '{UserTechnicianId}');");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
