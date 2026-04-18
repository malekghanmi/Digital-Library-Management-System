using Microsoft.EntityFrameworkCore;
using BibliothequeAPI.Models;

namespace BibliothequeAPI.Data;

public class BibliothequeContext : DbContext
{
    public BibliothequeContext(DbContextOptions<BibliothequeContext> options)
        : base(options) { }

    public DbSet<Livre> Livres { get; set; }
    public DbSet<Utilisateur> Utilisateurs { get; set; }
    public DbSet<Emprunt> Emprunts { get; set; }
    public DbSet<Article> Articles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Livre
        modelBuilder.Entity<Livre>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titre).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Auteur).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.ISBN).IsUnique().HasFilter("[ISBN] IS NOT NULL");
        });

        // Utilisateur
        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Emprunt
        modelBuilder.Entity<Emprunt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PenaliteRetard).HasColumnType("decimal(18,2)"); // ✅ CORRECTION
            entity.HasOne(e => e.Livre)
                  .WithMany(l => l.Emprunts)
                  .HasForeignKey(e => e.LivreId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Utilisateur)
                  .WithMany(u => u.Emprunts)
                  .HasForeignKey(e => e.UtilisateurId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Article
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titre).IsRequired().HasMaxLength(300);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Livre>().HasData(
            new Livre { Id = 1, Titre = "Le Petit Prince", Auteur = "Antoine de Saint-Exupéry", ISBN = "9782070612758", Genre = "Roman", Categorie = "Littérature", AnneePublication = 1943, NombreExemplaires = 3, ExemplairesDisponibles = 3, Description = "Un chef-d'œuvre de la littérature mondiale." },
            new Livre { Id = 2, Titre = "Clean Code", Auteur = "Robert C. Martin", ISBN = "9780132350884", Genre = "Informatique", Categorie = "Développement", AnneePublication = 2008, NombreExemplaires = 2, ExemplairesDisponibles = 2, Description = "Guide de bonnes pratiques en développement logiciel." },
            new Livre { Id = 3, Titre = "Design Patterns", Auteur = "Gang of Four", ISBN = "9780201633610", Genre = "Informatique", Categorie = "Architecture", AnneePublication = 1994, NombreExemplaires = 2, ExemplairesDisponibles = 2, Description = "Les patrons de conception incontournables." }
        );

        modelBuilder.Entity<Utilisateur>().HasData(
            new Utilisateur { Id = 1, Nom = "Admin", Prenom = "Bibliothèque", Email = "admin@biblio.com", Role = "Admin", DateInscription = new DateTime(2024, 1, 1) },
            new Utilisateur { Id = 2, Nom = "Dupont", Prenom = "Marie", Email = "marie.dupont@mail.com", Role = "Membre", DateInscription = new DateTime(2024, 1, 15) },
            new Utilisateur { Id = 3, Nom = "Martin", Prenom = "Jean", Email = "jean.martin@mail.com", Role = "Membre", DateInscription = new DateTime(2024, 2, 1) }
        );

        modelBuilder.Entity<Article>().HasData(
            new Article { Id = 1, Titre = "Introduction au Machine Learning", Auteur = "Dr. Ahmed Ben Ali", Revue = "Journal of AI Research", AnneePublication = 2023, Domaine = "Intelligence Artificielle", MotsCles = "ML, Deep Learning, Neural Networks", Resume = "Une introduction complète aux techniques d'apprentissage automatique." },
            new Article { Id = 2, Titre = "Microservices Architecture", Auteur = "Dr. Sophie Laurent", Revue = "IEEE Software", AnneePublication = 2023, Domaine = "Architecture Logicielle", MotsCles = "Microservices, Docker, Kubernetes", Resume = "Analyse des architectures microservices modernes." }
        );
    }
}