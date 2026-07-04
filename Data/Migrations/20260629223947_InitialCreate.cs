using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentStatuses",
                columns: table => new
                {
                    EquipmentStatusID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentStatuses", x => x.EquipmentStatusID);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTypes",
                columns: table => new
                {
                    EquipmentTypeID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypes", x => x.EquipmentTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    PersonID = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NationaleCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CellPhone = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    DateCreate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.PersonID);
                });

            migrationBuilder.CreateTable(
                name: "TransactionModes",
                columns: table => new
                {
                    TransactionModeID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionModes", x => x.TransactionModeID);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    EquipmentID = table.Column<Guid>(type: "TEXT", nullable: false),
                    EquipmentAssetNumber = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    EquipmentTypeID = table.Column<Guid>(type: "TEXT", nullable: false),
                    EquipmentStatusID = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateCreate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.EquipmentID);
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentStatuses_EquipmentStatusID",
                        column: x => x.EquipmentStatusID,
                        principalTable: "EquipmentStatuses",
                        principalColumn: "EquipmentStatusID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentTypes_EquipmentTypeID",
                        column: x => x.EquipmentTypeID,
                        principalTable: "EquipmentTypes",
                        principalColumn: "EquipmentTypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionID = table.Column<Guid>(type: "TEXT", nullable: false),
                    PersonID = table.Column<Guid>(type: "TEXT", nullable: false),
                    TransactionModeID = table.Column<Guid>(type: "TEXT", nullable: false),
                    EquipmentID = table.Column<Guid>(type: "TEXT", nullable: false),
                    EquipmentStatusID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    DateCreate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionID);
                    table.ForeignKey(
                        name: "FK_Transactions_EquipmentTypes_EquipmentStatusID",
                        column: x => x.EquipmentStatusID,
                        principalTable: "EquipmentTypes",
                        principalColumn: "EquipmentTypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Equipments_EquipmentID",
                        column: x => x.EquipmentID,
                        principalTable: "Equipments",
                        principalColumn: "EquipmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Persons_PersonID",
                        column: x => x.PersonID,
                        principalTable: "Persons",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_TransactionModes_TransactionModeID",
                        column: x => x.TransactionModeID,
                        principalTable: "TransactionModes",
                        principalColumn: "TransactionModeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentAssetNumber",
                table: "Equipments",
                column: "EquipmentAssetNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentStatusID",
                table: "Equipments",
                column: "EquipmentStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentTypeID",
                table: "Equipments",
                column: "EquipmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CellPhone",
                table: "Persons",
                column: "CellPhone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_NationaleCode",
                table: "Persons",
                column: "NationaleCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_EquipmentID",
                table: "Transactions",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_EquipmentStatusID",
                table: "Transactions",
                column: "EquipmentStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PersonID",
                table: "Transactions",
                column: "PersonID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionModeID",
                table: "Transactions",
                column: "TransactionModeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "TransactionModes");

            migrationBuilder.DropTable(
                name: "EquipmentStatuses");

            migrationBuilder.DropTable(
                name: "EquipmentTypes");
        }
    }
}
