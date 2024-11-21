using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coinnect.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    ContactUsId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContactUsText = table.Column<string>(type: "TEXT", nullable: true),
                    DateSent = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClientName = table.Column<string>(type: "TEXT", nullable: true),
                    ClientEmail = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.ContactUsId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
