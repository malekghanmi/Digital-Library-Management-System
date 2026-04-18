using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliothequeAPI.Data;
using BibliothequeAPI.DTOs;
using BibliothequeAPI.Models;

namespace BibliothequeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LivresController : ControllerBase
{
    private readonly BibliothequeContext _context;

    public LivresController(BibliothequeContext context)
    {
        _context = context;
    }

    // GET: api/livres
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LivreDto>>> GetLivres(
        [FromQuery] string? recherche,
        [FromQuery] string? genre,
        [FromQuery] string? categorie,
        [FromQuery] bool? disponible,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.Livres.Where(l => l.EstActif).AsQueryable();

        if (!string.IsNullOrWhiteSpace(recherche))
            query = query.Where(l => l.Titre.Contains(recherche) || l.Auteur.Contains(recherche) || (l.ISBN != null && l.ISBN.Contains(recherche)));

        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(l => l.Genre == genre);

        if (!string.IsNullOrWhiteSpace(categorie))
            query = query.Where(l => l.Categorie == categorie);

        if (disponible.HasValue && disponible.Value)
            query = query.Where(l => l.ExemplairesDisponibles > 0);

        var livres = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => MapToDto(l))
            .ToListAsync();

        return Ok(livres);
    }

    // GET: api/livres/5
    [HttpGet("{id}")]
    public async Task<ActionResult<LivreDto>> GetLivre(int id)
    {
        var livre = await _context.Livres.FindAsync(id);
        if (livre == null || !livre.EstActif)
            return NotFound(new { message = "Livre non trouvé." });

        return Ok(MapToDto(livre));
    }

    // POST: api/livres
    [HttpPost]
    public async Task<ActionResult<LivreDto>> CreateLivre(CreateLivreDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.ISBN) && await _context.Livres.AnyAsync(l => l.ISBN == dto.ISBN))
            return Conflict(new { message = "Un livre avec cet ISBN existe déjà." });

        var livre = new Livre
        {
            Titre = dto.Titre,
            Auteur = dto.Auteur,
            ISBN = dto.ISBN,
            Editeur = dto.Editeur,
            AnneePublication = dto.AnneePublication,
            Genre = dto.Genre,
            Categorie = dto.Categorie,
            NombreExemplaires = dto.NombreExemplaires,
            ExemplairesDisponibles = dto.NombreExemplaires,
            Description = dto.Description,
            ImageCouverture = dto.ImageCouverture
        };

        _context.Livres.Add(livre);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLivre), new { id = livre.Id }, MapToDto(livre));
    }

    // PUT: api/livres/5
    [HttpPut("{id}")]
    public async Task<ActionResult<LivreDto>> UpdateLivre(int id, UpdateLivreDto dto)
    {
        var livre = await _context.Livres.FindAsync(id);
        if (livre == null)
            return NotFound(new { message = "Livre non trouvé." });

        livre.Titre = dto.Titre;
        livre.Auteur = dto.Auteur;
        livre.ISBN = dto.ISBN;
        livre.Editeur = dto.Editeur;
        livre.AnneePublication = dto.AnneePublication;
        livre.Genre = dto.Genre;
        livre.Categorie = dto.Categorie;
        livre.NombreExemplaires = dto.NombreExemplaires;
        livre.Description = dto.Description;
        livre.ImageCouverture = dto.ImageCouverture;
        livre.EstActif = dto.EstActif;

        await _context.SaveChangesAsync();
        return Ok(MapToDto(livre));
    }

    // DELETE: api/livres/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLivre(int id)
    {
        var livre = await _context.Livres.FindAsync(id);
        if (livre == null)
            return NotFound(new { message = "Livre non trouvé." });

        livre.EstActif = false; // Soft delete
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // GET: api/livres/genres
    [HttpGet("genres")]
    public async Task<ActionResult<IEnumerable<string>>> GetGenres()
    {
        var genres = await _context.Livres
            .Where(l => l.Genre != null && l.EstActif)
            .Select(l => l.Genre!)
            .Distinct()
            .OrderBy(g => g)
            .ToListAsync();
        return Ok(genres);
    }

    private static LivreDto MapToDto(Livre l) => new LivreDto
    {
        Id = l.Id,
        Titre = l.Titre,
        Auteur = l.Auteur,
        ISBN = l.ISBN,
        Editeur = l.Editeur,
        AnneePublication = l.AnneePublication,
        Genre = l.Genre,
        Categorie = l.Categorie,
        NombreExemplaires = l.NombreExemplaires,
        ExemplairesDisponibles = l.ExemplairesDisponibles,
        Description = l.Description,
        ImageCouverture = l.ImageCouverture,
        DateAjout = l.DateAjout,
        EstActif = l.EstActif
    };
}
