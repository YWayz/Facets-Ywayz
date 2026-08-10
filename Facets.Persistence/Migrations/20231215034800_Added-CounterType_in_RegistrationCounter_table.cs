using System;
using Microsoft.EntityFrameworkCore.Migrations;
using static Facets.SharedKernal.AppEnums;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedCounterType_in_RegistrationCounter_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CounterType",
                table: "VisitorRegistrationCounter",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: nameof(CounterType.RegistrationAndPayment))
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "VisitorRegistrationCounterHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("b74ddd14-6340-4840-95c2-db12554843e5"),
                column: "PasswordHash",
                value: "AQAAAAIAB9AAAAAAEEBu3FWgRqgJWOiqjbX1yywHztopQIdUS0I23BsyaM8FxqlUionKPmAWRrD7obg/DQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CounterType",
                table: "VisitorRegistrationCounter")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "VisitorRegistrationCounterHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("b74ddd14-6340-4840-95c2-db12554843e5"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEJZHh/S5hmTm+8BR8ssy2GyMm04koddmCJLLGetMIWDEwKTXVwjow5mnIKwK5ExMNA==");
        }
    }
}
