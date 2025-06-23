using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventDrivenArchDemo.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddClientToRent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Rents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rents_ClientId",
                table: "Rents",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rents_Clients_ClientId",
                table: "Rents",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rents_Clients_ClientId",
                table: "Rents");

            migrationBuilder.DropIndex(
                name: "IX_Rents_ClientId",
                table: "Rents");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Rents");
        }
    }
}
