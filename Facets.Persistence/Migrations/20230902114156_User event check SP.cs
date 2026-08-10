using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UsereventcheckSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var text = @"
CREATE PROCEDURE [dbo].[SP_GetUserEvents] 
                        @UserProfileId  NVARCHAR(100)

                        AS
                            BEGIN  
								SELECT [u].[EventId]
								FROM [UserAssignedEvent] AS [u]
								WHERE [u].[UserProfileId] = @UserProfileId
	                        END
";

            migrationBuilder.Sql(text);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
