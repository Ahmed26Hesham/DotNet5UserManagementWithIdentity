using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManagmentWithIdentity.Data.Migrations
{
    public partial class AddAdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.Sql("INSERT INTO [security].[Users] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [LastName], [ProfilePicture]) VALUES (N'9a337156-f1d1-4ad8-851e-a0c521f7b643', N'admin', N'ADMIN', N'admin@test.com', N'ADMIN@TEST.COM', 0, N'AQAAAAEAACcQAAAAEC7x1RaJ4LWS8u34sZdfI2CIQTWGY6nxMpFgzYUMcrw/RgYpnZExAnuUaPAASuH5UA==', N'OW5FVOUO3L4QKUMH7W7PNBET3GAHWRX4', N'f175e5bd-ef15-41e3-b04d-02d4c5ba2c75', NULL, 0, 0, NULL, 1, 0, N'mohamed', N'hesham', null)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [security].[Users] WHERE Id ='9a337156-f1d1-4ad8-851e-a0c521f7b643' ");
        }
    }
}
