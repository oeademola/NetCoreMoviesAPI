using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesAPI.Migrations
{
    public partial class AdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            Insert into AspNetRoles (Id, [name], [NormalizedName])
            values ('02938a57-c5a1-4ddd-97a0-46701d246137', 'Admin', 'Admin')
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete AspNetRoles
            where id = '02938a57-c5a1-4ddd-97a0-46701d246137'
            ");
        }
    }
}
