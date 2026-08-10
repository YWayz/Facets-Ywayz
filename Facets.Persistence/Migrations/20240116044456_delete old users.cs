using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class deleteoldusers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"
                        UPDATE Visitor  
                        SET IsDeleted = 1 
                        WHERE NICNumber like '%old%' or PassportNumber like '%old%' or NICNumber  like '%inv%' or PassportNumber like '%inv%'
                       ";

            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
