using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesAPI.Migrations
{
    public partial class AdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT [dbo].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'0420ce6e-471b-4dc5-bf29-a11549ba9fcc', N'seyi@gmail.com', N'SEYI@GMAIL.COM', N'seyi@gmail.com', N'SEYI@GMAIL.COM', 0, N'AQAAAAEAACcQAAAAEGbtNXMLKY0GEqyMxrXosl9x5JhhD1K9zA6PMXg7C+MoCHNLkYkXd5q0GVHNfouKdA==', N'NH6CCWA2D7ODCLOQH5S6ZIPZPU627HVP', N'385af41b-226d-43ed-bdbc-21453e5efd33', NULL, 0, 0, NULL, 1, 0)
                GO
                SET IDENTITY_INSERT [dbo].[AspNetUserClaims] ON 
                GO
                INSERT [dbo].[AspNetUserClaims] ([Id], [UserId], [ClaimType], [ClaimValue]) VALUES (1, N'0420ce6e-471b-4dc5-bf29-a11549ba9fcc', N'http://schemas.microsoft.com/ws/2008/06/identity/claims/role', N'Admin')
                GO
                SET IDENTITY_INSERT [dbo].[AspNetUserClaims] OFF
                GO
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            delete [dbo].[AspNetUsers] where [Id] = '0420ce6e-471b-4dc5-bf29-a11549ba9fcc'
            delete [dbo].[AspNetUserClaims] where [Id] = 1
            ");
        }
    }
}
