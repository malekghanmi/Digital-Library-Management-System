using System.ComponentModel.DataAnnotations;

namespace BibliothequeAPI.Models;

public class Article
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string Titre { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Auteur { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Revue { get; set; }

    public int? AnneePublication { get; set; }

    [MaxLength(100)]
    public string? Domaine { get; set; }

    [MaxLength(50)]
    public string? DOI { get; set; }

    [MaxLength(3000)]
    public string? Resume { get; set; }

    [MaxLength(500)]
    public string? MotsCles { get; set; }

    public string? LienAcces { get; set; }

    public DateTime DateAjout { get; set; } = DateTime.UtcNow;

    public bool EstActif { get; set; } = true;
}
