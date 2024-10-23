using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCheckoutViewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MemberId1",
                table: "Checkouts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_MemberId1",
                table: "Checkouts",
                column: "MemberId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_Users_MemberId1",
                table: "Checkouts",
                column: "MemberId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_Users_MemberId1",
                table: "Checkouts");

            migrationBuilder.DropIndex(
                name: "IX_Checkouts_MemberId1",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "MemberId1",
                table: "Checkouts");
        }
    }
}
