using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliothequeAPI.Data;
using BibliothequeAPI.DTOs;

namespace BibliothequeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatistiquesController : ControllerBase
{
    private readonly BibliothequeContext _context;

    public StatistiquesController(BibliothequeContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<StatistiquesDto>> GetStatistiques()
    {
        var stats = new StatistiquesDto
        {
            TotalLivres = await _context.Livres.CountAsync(l => l.EstActif),
            TotalUtilisateurs = await _context.Utilisateurs.CountAsync(u => u.EstActif),
            TotalEmpruntsActifs = await _context.Emprunts.CountAsync(e => e.Statut == "EnCours"),
            TotalEmpruntsEnRetard = await _context.Emprunts.CountAsync(e => e.Statut == "EnRetard"),
            TotalArticles = await _context.Articles.CountAsync(a => a.EstActif),
            LivresDisponibles = await _context.Livres.CountAsync(l => l.EstActif && l.ExemplairesDisponibles > 0)
        };

        return Ok(stats);
    }
}

[ApiController]
[Route("api/[controller]")]
public class RechercheController : ControllerBase
{
    private readonly BibliothequeContext _context;

    public RechercheController(BibliothequeContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<RechercheResultatDto>> Rechercher([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { message = "Le terme de recherche est requis." });

        var livres = await _context.Livres
            .Where(l => l.EstActif && (l.Titre.Contains(q) || l.Auteur.Contains(q) ||
                                        (l.Genre != null && l.Genre.Contains(q)) ||
                                        (l.Description != null && l.Description.Contains(q))))
            .Take(20)
            .Select(l => new LivreDto
            {
                Id = l.Id, Titre = l.Titre, Auteur = l.Auteur, ISBN = l.ISBN,
                Genre = l.Genre, Categorie = l.Categorie, AnneePublication = l.AnneePublication,
                ExemplairesDisponibles = l.ExemplairesDisponibles, NombreExemplaires = l.NombreExemplaires,
                Description = l.Description, DateAjout = l.DateAjout, EstActif = l.EstActif
            })
            .ToListAsync();

        var articles = await _context.Articles
            .Where(a => a.EstActif && (a.Titre.Contains(q) || a.Auteur.Contains(q) ||
                                        (a.MotsCles != null && a.MotsCles.Contains(q)) ||
                                        (a.Resume != null && a.Resume.Contains(q))))
            .Take(20)
            .Select(a => new ArticleDto
            {
                Id = a.Id, Titre = a.Titre, Auteur = a.Auteur, Revue = a.Revue,
                AnneePublication = a.AnneePublication, Domaine = a.Domaine,
                MotsCles = a.MotsCles, Resume = a.Resume, DateAjout = a.DateAjout, EstActif = a.EstActif
            })
            .ToListAsync();

        return Ok(new RechercheResultatDto
        {
            Livres = livres,
            Articles = articles,
            TotalLivres = livres.Count,
            TotalArticles = articles.Count
        });
    }
}
