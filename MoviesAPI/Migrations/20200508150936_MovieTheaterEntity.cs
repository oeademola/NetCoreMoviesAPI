using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Migrations
{
    public partial class MovieTheaterEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovieTheaters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Location = table.Column<Point>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieTheaters", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "MovieTheaters",
                columns: new[] { "Id", "Location", "Name" },
                values: new object[,]
                {
                    { 1, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (3.3154061 6.8294238)"), "SilverBird Cinema" },
                    { 2, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (3.5069803 6.4375493)"), "Lekki Film House" },
                    { 3, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (3.3393472 6.5062713)"), "Ozone Cinema" },
                    { 4, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-7.4792365 53.5731389)"), "CineWorld" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieTheaters");
        }
    }
}
