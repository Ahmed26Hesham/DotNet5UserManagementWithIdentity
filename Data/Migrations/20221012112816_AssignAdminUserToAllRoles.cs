using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManagmentWithIdentity.Data.Migrations
{
    public partial class AssignAdminUserToAllRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT into [security].[UserRoles] (UserId , RoleId) SELECT '9a337156-f1d1-4ad8-851e-a0c521f7b643' , Id FROM [security].[Roles]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE from [security].[UserRoles] WHERE UserId = '9a337156-f1d1-4ad8-851e-a0c521f7b643'");
        }
    }
}
