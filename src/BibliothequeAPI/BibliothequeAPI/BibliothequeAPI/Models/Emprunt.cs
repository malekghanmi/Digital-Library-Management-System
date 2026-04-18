using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliothequeAPI.Models;

public class Emprunt
{
    public int Id { get; set; }

    [Required]
    public int LivreId { get; set; }

    [Required]
    public int UtilisateurId { get; set; }

    public DateTime DateEmprunt { get; set; } = DateTime.UtcNow;

    public DateTime DateRetourPrevue { get; set; }

    public DateTime? DateRetourReelle { get; set; }

    [MaxLength(50)]
    public string Statut { get; set; } = "EnCours"; // EnCours, Retourne, EnRetard

    [MaxLength(500)]
    public string? Notes { get; set; }

    public decimal? PenaliteRetard { get; set; }

    // Navigation properties
    [ForeignKey("LivreId")]
    public Livre? Livre { get; set; }

    [ForeignKey("UtilisateurId")]
    public Utilisateur? Utilisateur { get; set; }
}
