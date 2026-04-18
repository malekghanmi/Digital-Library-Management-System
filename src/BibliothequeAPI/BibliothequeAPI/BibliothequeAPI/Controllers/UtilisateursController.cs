using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliothequeAPI.Data;
using BibliothequeAPI.DTOs;
using BibliothequeAPI.Models;

namespace BibliothequeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UtilisateursController : ControllerBase
{
    private readonly BibliothequeContext _context;

    public UtilisateursController(BibliothequeContext context)
    {
        _context = context;
    }

    // GET: api/utilisateurs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UtilisateurDto>>> GetUtilisateurs(
        [FromQuery] string? recherche,
        [FromQuery] string? role,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.Utilisateurs.Include(u => u.Emprunts).Where(u => u.EstActif).AsQueryable();

        if (!string.IsNullOrWhiteSpace(recherche))
            query = query.Where(u => u.Nom.Contains(recherche) || u.Prenom.Contains(recherche) || u.Email.Contains(recherche));

        if (!string.IsNullOrWhiteSpace(role))
            query = query.Where(u => u.Role == role);

        var utilisateurs = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(utilisateurs.Select(u => MapToDto(u)));
    }

    // GET: api/utilisateurs/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UtilisateurDto>> GetUtilisateur(int id)
    {
        var u = await _context.Utilisateurs.Include(u => u.Emprunts).FirstOrDefaultAsync(u => u.Id == id);
        if (u == null || !u.EstActif)
            return NotFound(new { message = "Utilisateur non trouvé." });

        return Ok(MapToDto(u));
    }

    // POST: api/utilisateurs
    [HttpPost]
    public async Task<ActionResult<UtilisateurDto>> CreateUtilisateur(CreateUtilisateurDto dto)
    {
        if (await _context.Utilisateurs.AnyAsync(u => u.Email == dto.Email))
            return Conflict(new { message = "Un utilisateur avec cet email existe déjà." });

        var utilisateur = new Utilisateur
        {
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            Email = dto.Email,
            Telephone = dto.Telephone,
            Adresse = dto.Adresse,
            DateNaissance = dto.DateNaissance,
            Role = dto.Role,
            MotDePasseHash = dto.MotDePasse != null ? BCryptHash(dto.MotDePasse) : null
        };

        _context.Utilisateurs.Add(utilisateur);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUtilisateur), new { id = utilisateur.Id }, MapToDto(utilisateur));
    }

    // PUT: api/utilisateurs/5
    [HttpPut("{id}")]
    public async Task<ActionResult<UtilisateurDto>> UpdateUtilisateur(int id, UpdateUtilisateurDto dto)
    {
        var utilisateur = await _context.Utilisateurs.FindAsync(id);
        if (utilisateur == null)
            return NotFound(new { message = "Utilisateur non trouvé." });

        if (await _context.Utilisateurs.AnyAsync(u => u.Email == dto.Email && u.Id != id))
            return Conflict(new { message = "Cet email est déjà utilisé." });

        utilisateur.Nom = dto.Nom;
        utilisateur.Prenom = dto.Prenom;
        utilisateur.Email = dto.Email;
        utilisateur.Telephone = dto.Telephone;
        utilisateur.Adresse = dto.Adresse;
        utilisateur.DateNaissance = dto.DateNaissance;
        utilisateur.Role = dto.Role;
        utilisateur.EstActif = dto.EstActif;

        await _context.SaveChangesAsync();
        return Ok(MapToDto(utilisateur));
    }

    // DELETE: api/utilisateurs/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUtilisateur(int id)
    {
        var utilisateur = await _context.Utilisateurs.FindAsync(id);
        if (utilisateur == null)
            return NotFound(new { message = "Utilisateur non trouvé." });

        utilisateur.EstActif = false;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static string BCryptHash(string password) =>
        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password)); // Simplified - use BCrypt in production

    private static UtilisateurDto MapToDto(Utilisateur u) => new UtilisateurDto
    {
        Id = u.Id,
        Nom = u.Nom,
        Prenom = u.Prenom,
        Email = u.Email,
        Telephone = u.Telephone,
        Adresse = u.Adresse,
        DateInscription = u.DateInscription,
        DateNaissance = u.DateNaissance,
        Role = u.Role,
        EstActif = u.EstActif,
        NombreEmpruntsActifs = u.Emprunts.Count(e => e.Statut == "EnCours")
    };
}
