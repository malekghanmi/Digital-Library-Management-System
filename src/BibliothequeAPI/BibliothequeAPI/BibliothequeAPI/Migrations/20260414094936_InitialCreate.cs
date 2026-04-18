using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BibliothequeAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titre = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Auteur = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Revue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AnneePublication = table.Column<int>(type: "int", nullable: true),
                    Domaine = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DOI = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Resume = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    MotsCles = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LienAcces = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAjout = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstActif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Livres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titre = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Auteur = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Editeur = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AnneePublication = table.Column<int>(type: "int", nullable: true),
                    Genre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Categorie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NombreExemplaires = table.Column<int>(type: "int", nullable: false),
                    ExemplairesDisponibles = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ImageCouverture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAjout = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstActif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DateInscription = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateNaissance = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    MotDePasseHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Emprunts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LivreId = table.Column<int>(type: "int", nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false),
                    DateEmprunt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateRetourPrevue = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateRetourReelle = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PenaliteRetard = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emprunts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Emprunts_Livres_LivreId",
                        column: x => x.LivreId,
                        principalTable: "Livres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Emprunts_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Articles",
                columns: new[] { "Id", "AnneePublication", "Auteur", "DOI", "DateAjout", "Domaine", "EstActif", "LienAcces", "MotsCles", "Resume", "Revue", "Titre" },
                values: new object[,]
                {
                    { 1, 2023, "Dr. Ahmed Ben Ali", null, new DateTime(2026, 4, 14, 9, 49, 36, 440, DateTimeKind.Utc).AddTicks(5678), "Intelligence Artificielle", true, null, "ML, Deep Learning, Neural Networks", "Une introduction complète aux techniques d'apprentissage automatique.", "Journal of AI Research", "Introduction au Machine Learning" },
                    { 2, 2023, "Dr. Sophie Laurent", null, new DateTime(2026, 4, 14, 9, 49, 36, 440, DateTimeKind.Utc).AddTicks(6684), "Architecture Logicielle", true, null, "Microservices, Docker, Kubernetes", "Analyse des architectures microservices modernes.", "IEEE Software", "Microservices Architecture" }
                });

            migrationBuilder.InsertData(
                table: "Livres",
                columns: new[] { "Id", "AnneePublication", "Auteur", "Categorie", "DateAjout", "Description", "Editeur", "EstActif", "ExemplairesDisponibles", "Genre", "ISBN", "ImageCouverture", "NombreExemplaires", "Titre" },
                values: new object[,]
                {
                    { 1, 1943, "Antoine de Saint-Exupéry", "Littérature", new DateTime(2026, 4, 14, 9, 49, 36, 439, DateTimeKind.Utc).AddTicks(8785), "Un chef-d'œuvre de la littérature mondiale.", null, true, 3, "Roman", "9782070612758", null, 3, "Le Petit Prince" },
                    { 2, 2008, "Robert C. Martin", "Développement", new DateTime(2026, 4, 14, 9, 49, 36, 440, DateTimeKind.Utc).AddTicks(227), "Guide de bonnes pratiques en développement logiciel.", null, true, 2, "Informatique", "9780132350884", null, 2, "Clean Code" },
                    { 3, 1994, "Gang of Four", "Architecture", new DateTime(2026, 4, 14, 9, 49, 36, 440, DateTimeKind.Utc).AddTicks(230), "Les patrons de conception incontournables.", null, true, 2, "Informatique", "9780201633610", null, 2, "Design Patterns" }
                });

            migrationBuilder.InsertData(
                table: "Utilisateurs",
                columns: new[] { "Id", "Adresse", "DateInscription", "DateNaissance", "Email", "EstActif", "MotDePasseHash", "Nom", "Prenom", "Role", "Telephone" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "admin@biblio.com", true, null, "Admin", "Bibliothèque", "Admin", null },
                    { 2, null, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "marie.dupont@mail.com", true, null, "Dupont", "Marie", "Membre", null },
                    { 3, null, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "jean.martin@mail.com", true, null, "Martin", "Jean", "Membre", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emprunts_LivreId",
                table: "Emprunts",
                column: "LivreId");

            migrationBuilder.CreateIndex(
                name: "IX_Emprunts_UtilisateurId",
                table: "Emprunts",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Livres_ISBN",
                table: "Livres",
                column: "ISBN",
                unique: true,
                filter: "[ISBN] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Email",
                table: "Utilisateurs",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Emprunts");

            migrationBuilder.DropTable(
                name: "Livres");

            migrationBuilder.DropTable(
                name: "Utilisateurs");
        }
    }
}
