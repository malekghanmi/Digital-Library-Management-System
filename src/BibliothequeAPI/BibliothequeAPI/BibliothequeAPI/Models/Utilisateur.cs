using System.ComponentModel.DataAnnotations;

namespace BibliothequeAPI.Models;

public class Utilisateur
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Prenom { get; set; } = string.Empty;

    [Required, MaxLength(200), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Telephone { get; set; }

    [MaxLength(300)]
    public string? Adresse { get; set; }

    public DateTime DateInscription { get; set; } = DateTime.UtcNow;

    public DateTime? DateNaissance { get; set; }

    [MaxLength(50)]
    public string Role { get; set; } = "Membre"; // Membre, Bibliothécaire, Admin

    public bool EstActif { get; set; } = true;

    [MaxLength(200)]
    public string? MotDePasseHash { get; set; }

    // Relations
    public ICollection<Emprunt> Emprunts { get; set; } = new List<Emprunt>();
}
