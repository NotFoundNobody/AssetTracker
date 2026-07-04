using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class AddPersonForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PersonID",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PersonID",
                table: "Users",
                column: "PersonID");

            migrationBuilder.Sql(
                "PRAGMA foreign_keys = OFF;",
                suppressTransaction: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Persons_PersonID",
                table: "Users",
                column: "PersonID",
                principalTable: "Persons",
                principalColumn: "PersonID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(
                "PRAGMA foreign_keys = ON;",
                suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "PRAGMA foreign_keys = OFF;",
                suppressTransaction: true);

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Persons_PersonID",
                table: "Users");

            migrationBuilder.Sql(
                "PRAGMA foreign_keys = ON;",
                suppressTransaction: true);

            migrationBuilder.DropIndex(
                name: "IX_Users_PersonID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PersonID",
                table: "Users");
        }
    }
}

