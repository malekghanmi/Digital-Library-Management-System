using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliothequeAPI.Models;

public class Livre
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string Titre { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Auteur { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? ISBN { get; set; }

    [MaxLength(100)]
    public string? Editeur { get; set; }

    public int? AnneePublication { get; set; }

    [MaxLength(100)]
    public string? Genre { get; set; }

    [MaxLength(100)]
    public string? Categorie { get; set; }

    public int NombreExemplaires { get; set; } = 1;

    public int ExemplairesDisponibles { get; set; } = 1;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public string? ImageCouverture { get; set; }

    public DateTime DateAjout { get; set; } = DateTime.UtcNow;

    public bool EstActif { get; set; } = true;

    // Relations
    public ICollection<Emprunt> Emprunts { get; set; } = new List<Emprunt>();
}
