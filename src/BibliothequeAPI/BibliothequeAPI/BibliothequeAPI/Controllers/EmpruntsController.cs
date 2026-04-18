using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliothequeAPI.Data;
using BibliothequeAPI.DTOs;
using BibliothequeAPI.Models;

namespace BibliothequeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpruntsController : ControllerBase
{
    private readonly BibliothequeContext _context;

    public EmpruntsController(BibliothequeContext context)
    {
        _context = context;
    }

    // GET: api/emprunts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmpruntDto>>> GetEmprunts(
        [FromQuery] string? statut,
        [FromQuery] int? utilisateurId,
        [FromQuery] int? livreId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        // Mise à jour automatique des retards
        await MettreAJourRetards();

        var query = _context.Emprunts
            .Include(e => e.Livre)
            .Include(e => e.Utilisateur)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(statut))
            query = query.Where(e => e.Statut == statut);

        if (utilisateurId.HasValue)
            query = query.Where(e => e.UtilisateurId == utilisateurId.Value);

        if (livreId.HasValue)
            query = query.Where(e => e.LivreId == livreId.Value);

        var emprunts = await query
            .OrderByDescending(e => e.DateEmprunt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(emprunts.Select(e => MapToDto(e)));
    }

    // GET: api/emprunts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EmpruntDto>> GetEmprunt(int id)
    {
        var emprunt = await _context.Emprunts
            .Include(e => e.Livre)
            .Include(e => e.Utilisateur)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (emprunt == null)
            return NotFound(new { message = "Emprunt non trouvé." });

        return Ok(MapToDto(emprunt));
    }

    // POST: api/emprunts
    [HttpPost]
    public async Task<ActionResult<EmpruntDto>> CreateEmprunt(CreateEmpruntDto dto)
    {
        var livre = await _context.Livres.FindAsync(dto.LivreId);
        if (livre == null || !livre.EstActif)
            return NotFound(new { message = "Livre non trouvé." });

        if (livre.ExemplairesDisponibles <= 0)
            return BadRequest(new { message = "Aucun exemplaire disponible pour ce livre." });

        var utilisateur = await _context.Utilisateurs.FindAsync(dto.UtilisateurId);
        if (utilisateur == null || !utilisateur.EstActif)
            return NotFound(new { message = "Utilisateur non trouvé." });

        var emprunt = new Emprunt
        {
            LivreId = dto.LivreId,
            UtilisateurId = dto.UtilisateurId,
            DateRetourPrevue = dto.DateRetourPrevue,
            Notes = dto.Notes,
            Statut = "EnCours"
        };

        livre.ExemplairesDisponibles--;

        _context.Emprunts.Add(emprunt);
        await _context.SaveChangesAsync();

        var empruntComplet = await _context.Emprunts
            .Include(e => e.Livre)
            .Include(e => e.Utilisateur)
            .FirstAsync(e => e.Id == emprunt.Id);

        return CreatedAtAction(nameof(GetEmprunt), new { id = emprunt.Id }, MapToDto(empruntComplet));
    }

    // PUT: api/emprunts/retour
    [HttpPut("retour")]
    public async Task<ActionResult<EmpruntDto>> RetournerLivre(RetourEmpruntDto dto)
    {
        var emprunt = await _context.Emprunts
            .Include(e => e.Livre)
            .Include(e => e.Utilisateur)
            .FirstOrDefaultAsync(e => e.Id == dto.EmpruntId);

        if (emprunt == null)
            return NotFound(new { message = "Emprunt non trouvé." });

        if (emprunt.Statut == "Retourne")
            return BadRequest(new { message = "Ce livre a déjà été retourné." });

        emprunt.DateRetourReelle = dto.DateRetourReelle;
        emprunt.Statut = "Retourne";
        emprunt.Notes = dto.Notes;

        // Calcul pénalité
        if (dto.DateRetourReelle > emprunt.DateRetourPrevue)
        {
            var joursRetard = (dto.DateRetourReelle - emprunt.DateRetourPrevue).Days;
            emprunt.PenaliteRetard = joursRetard * 0.50m; // 0.50€ par jour
        }

        if (emprunt.Livre != null)
            emprunt.Livre.ExemplairesDisponibles++;

        await _context.SaveChangesAsync();
        return Ok(MapToDto(emprunt));
    }

    private async Task MettreAJourRetards()
    {
        var empruntsEnRetard = await _context.Emprunts
            .Where(e => e.Statut == "EnCours" && e.DateRetourPrevue < DateTime.UtcNow)
            .ToListAsync();

        foreach (var e in empruntsEnRetard)
            e.Statut = "EnRetard";

        if (empruntsEnRetard.Any())
            await _context.SaveChangesAsync();
    }

    private static EmpruntDto MapToDto(Emprunt e) => new EmpruntDto
    {
        Id = e.Id,
        LivreId = e.LivreId,
        TitreLivre = e.Livre?.Titre ?? "",
        AuteurLivre = e.Livre?.Auteur ?? "",
        UtilisateurId = e.UtilisateurId,
        NomUtilisateur = e.Utilisateur?.Nom ?? "",
        PrenomUtilisateur = e.Utilisateur?.Prenom ?? "",
        DateEmprunt = e.DateEmprunt,
        DateRetourPrevue = e.DateRetourPrevue,
        DateRetourReelle = e.DateRetourReelle,
        Statut = e.Statut,
        Notes = e.Notes,
        PenaliteRetard = e.PenaliteRetard
    };
}
