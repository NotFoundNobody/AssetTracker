using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class DebugTransactionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_EquipmentTypes_EquipmentStatusID",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_EquipmentStatuses_EquipmentStatusID",
                table: "Transactions",
                column: "EquipmentStatusID",
                principalTable: "EquipmentStatuses",
                principalColumn: "EquipmentStatusID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_EquipmentStatuses_EquipmentStatusID",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_EquipmentTypes_EquipmentStatusID",
                table: "Transactions",
                column: "EquipmentStatusID",
                principalTable: "EquipmentTypes",
                principalColumn: "EquipmentTypeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
