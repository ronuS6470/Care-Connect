using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareConnect.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCaregiverDateOfBirthAndExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Caregiver_HourlyRate_NonNegative",
                table: "Caregivers");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Caregivers",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "Caregivers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Caregiver_DateOfBirth_NotInFuture",
                table: "Caregivers",
                sql: "[DateOfBirth] <= CAST(SYSUTCDATETIME() AS date)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Caregiver_HourlyRate_Positive",
                table: "Caregivers",
                sql: "[HourlyRate] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Caregiver_YearsOfExperience_NonNegative",
                table: "Caregivers",
                sql: "[YearsOfExperience] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Caregiver_DateOfBirth_NotInFuture",
                table: "Caregivers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Caregiver_HourlyRate_Positive",
                table: "Caregivers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Caregiver_YearsOfExperience_NonNegative",
                table: "Caregivers");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Caregivers");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "Caregivers");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Caregiver_HourlyRate_NonNegative",
                table: "Caregivers",
                sql: "[HourlyRate] >= 0");
        }
    }
}
