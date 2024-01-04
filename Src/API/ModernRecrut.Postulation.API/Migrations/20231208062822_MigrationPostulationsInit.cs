using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModernRecrut.Postulations.API.Migrations
{
    public partial class MigrationPostulationsInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Postulation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdCandidat = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdOffreEmploi = table.Column<Guid>(type: "TEXT", nullable: false),
                    PretentionSalariale = table.Column<decimal>(type: "TEXT", nullable: false),
                    DateDisponibilite = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postulation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Note",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdPostulation = table.Column<Guid>(type: "TEXT", nullable: false),
                    Contenu = table.Column<string>(type: "TEXT", nullable: true),
                    NomEmetteur = table.Column<string>(type: "TEXT", nullable: true),
                    PostulationId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Note", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Note_Postulation_PostulationId",
                        column: x => x.PostulationId,
                        principalTable: "Postulation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Note_PostulationId",
                table: "Note",
                column: "PostulationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Note");

            migrationBuilder.DropTable(
                name: "Postulation");
        }
    }
}
