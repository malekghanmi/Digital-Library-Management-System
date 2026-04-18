namespace BibliothequeFront.Models;

// ===== LIVRE =====
public class LivreViewModel
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Auteur { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public string? Editeur { get; set; }
    public int? AnneePublication { get; set; }
    public string? Genre { get; set; }
    public string? Categorie { get; set; }
    public int NombreExemplaires { get; set; }
    public int ExemplairesDisponibles { get; set; }
    public string? Description { get; set; }
    public string? ImageCouverture { get; set; }
    public DateTime DateAjout { get; set; }
    public bool EstActif { get; set; }
}

public class CreateLivreViewModel
{
    public string Titre { get; set; } = string.Empty;
    public string Auteur { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public string? Editeur { get; set; }
    public int? AnneePublication { get; set; }
    public string? Genre { get; set; }
    public string? Categorie { get; set; }
    public int NombreExemplaires { get; set; } = 1;
    public string? Description { get; set; }
    public string? ImageCouverture { get; set; }
}

// ===== UTILISATEUR =====
public class UtilisateurViewModel
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Adresse { get; set; }
    public DateTime DateInscription { get; set; }
    public DateTime? DateNaissance { get; set; }
    public string Role { get; set; } = "Membre";
    public bool EstActif { get; set; }
    public int NombreEmpruntsActifs { get; set; }
}

public class CreateUtilisateurViewModel
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Adresse { get; set; }
    public DateTime? DateNaissance { get; set; }
    public string Role { get; set; } = "Membre";
    public string? MotDePasse { get; set; }
}

// ===== EMPRUNT =====
public class EmpruntViewModel
{
    public int Id { get; set; }
    public int LivreId { get; set; }
    public string TitreLivre { get; set; } = string.Empty;
    public string AuteurLivre { get; set; } = string.Empty;
    public int UtilisateurId { get; set; }
    public string NomUtilisateur { get; set; } = string.Empty;
    public string PrenomUtilisateur { get; set; } = string.Empty;
    public DateTime DateEmprunt { get; set; }
    public DateTime DateRetourPrevue { get; set; }
    public DateTime? DateRetourReelle { get; set; }
    public string Statut { get; set; } = "EnCours";
    public string? Notes { get; set; }
    public decimal? PenaliteRetard { get; set; }
}

public class CreateEmpruntViewModel
{
    public int LivreId { get; set; }
    public int UtilisateurId { get; set; }
    public DateTime DateRetourPrevue { get; set; }
    public string? Notes { get; set; }
}

// ===== ARTICLE =====
public class ArticleViewModel
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Auteur { get; set; } = string.Empty;
    public string? Revue { get; set; }
    public int? AnneePublication { get; set; }
    public string? Domaine { get; set; }
    public string? DOI { get; set; }
    public string? Resume { get; set; }
    public string? MotsCles { get; set; }
    public string? LienAcces { get; set; }
    public DateTime DateAjout { get; set; }
    public bool EstActif { get; set; }
}

public class CreateArticleViewModel
{
    public string Titre { get; set; } = string.Empty;
    public string Auteur { get; set; } = string.Empty;
    public string? Revue { get; set; }
    public int? AnneePublication { get; set; }
    public string? Domaine { get; set; }
    public string? DOI { get; set; }
    public string? Resume { get; set; }
    public string? MotsCles { get; set; }
    public string? LienAcces { get; set; }
}

// ===== STATS =====
public class StatistiquesViewModel
{
    public int TotalLivres { get; set; }
    public int TotalUtilisateurs { get; set; }
    public int TotalEmpruntsActifs { get; set; }
    public int TotalEmpruntsEnRetard { get; set; }
    public int TotalArticles { get; set; }
    public int LivresDisponibles { get; set; }
}

// ===== RECHERCHE =====
public class RechercheResultatViewModel
{
    public List<LivreViewModel> Livres { get; set; } = new();
    public List<ArticleViewModel> Articles { get; set; } = new();
    public int TotalLivres { get; set; }
    public int TotalArticles { get; set; }
    public string? TermeRecherche { get; set; }
}
