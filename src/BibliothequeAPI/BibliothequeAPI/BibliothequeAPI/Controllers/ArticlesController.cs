using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliothequeAPI.Data;
using BibliothequeAPI.DTOs;
using BibliothequeAPI.Models;

namespace BibliothequeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly BibliothequeContext _context;

    public ArticlesController(BibliothequeContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles(
        [FromQuery] string? recherche,
        [FromQuery] string? domaine,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.Articles.Where(a => a.EstActif).AsQueryable();

        if (!string.IsNullOrWhiteSpace(recherche))
            query = query.Where(a => a.Titre.Contains(recherche) || a.Auteur.Contains(recherche) ||
                                     (a.MotsCles != null && a.MotsCles.Contains(recherche)));

        if (!string.IsNullOrWhiteSpace(domaine))
            query = query.Where(a => a.Domaine == domaine);

        var articles = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => MapToDto(a))
            .ToListAsync();

        return Ok(articles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArticleDto>> GetArticle(int id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article == null || !article.EstActif)
            return NotFound(new { message = "Article non trouvé." });
        return Ok(MapToDto(article));
    }

    [HttpPost]
    public async Task<ActionResult<ArticleDto>> CreateArticle(CreateArticleDto dto)
    {
        var article = new Article
        {
            Titre = dto.Titre,
            Auteur = dto.Auteur,
            Revue = dto.Revue,
            AnneePublication = dto.AnneePublication,
            Domaine = dto.Domaine,
            DOI = dto.DOI,
            Resume = dto.Resume,
            MotsCles = dto.MotsCles,
            LienAcces = dto.LienAcces
        };

        _context.Articles.Add(article);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, MapToDto(article));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ArticleDto>> UpdateArticle(int id, CreateArticleDto dto)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article == null)
            return NotFound(new { message = "Article non trouvé." });

        article.Titre = dto.Titre;
        article.Auteur = dto.Auteur;
        article.Revue = dto.Revue;
        article.AnneePublication = dto.AnneePublication;
        article.Domaine = dto.Domaine;
        article.DOI = dto.DOI;
        article.Resume = dto.Resume;
        article.MotsCles = dto.MotsCles;
        article.LienAcces = dto.LienAcces;

        await _context.SaveChangesAsync();
        return Ok(MapToDto(article));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article == null)
            return NotFound(new { message = "Article non trouvé." });
        article.EstActif = false;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("domaines")]
    public async Task<ActionResult<IEnumerable<string>>> GetDomaines()
    {
        var domaines = await _context.Articles
            .Where(a => a.Domaine != null && a.EstActif)
            .Select(a => a.Domaine!)
            .Distinct()
            .OrderBy(d => d)
            .ToListAsync();
        return Ok(domaines);
    }

    private static ArticleDto MapToDto(Article a) => new ArticleDto
    {
        Id = a.Id, Titre = a.Titre, Auteur = a.Auteur, Revue = a.Revue,
        AnneePublication = a.AnneePublication, Domaine = a.Domaine, DOI = a.DOI,
        Resume = a.Resume, MotsCles = a.MotsCles, LienAcces = a.LienAcces,
        DateAjout = a.DateAjout, EstActif = a.EstActif
    };
}
