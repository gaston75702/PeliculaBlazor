using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeliculaBlazor.Server.Migrations
{
    /// <inheritdoc />
    public partial class RolAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO AspNetRoles(Id,Name,NormalizedName)
                                  VALUES('2b5106f5-098a-445b-984f-22c239ac3c75','admin','ADMIN')");    
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE AspNetRoles WHERE Id='2b5106f5-098a-445b-984f-22c239ac3c75'");
        }
    }
}
