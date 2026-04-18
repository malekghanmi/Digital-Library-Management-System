using Microsoft.AspNetCore.Mvc;
using BibliothequeFront.Models;
using BibliothequeFront.Services;

namespace BibliothequeFront.Controllers;

public class EmpruntsController : Controller
{
    private readonly ApiService _api;

    public EmpruntsController(ApiService api) => _api = api;

    public async Task<IActionResult> Index(string? statut, int page = 1)
    {
        var emprunts = await _api.GetEmpruntsAsync(statut, page);
        ViewBag.Statut = statut;
        ViewBag.Page = page;
        return View(emprunts);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Livres = await _api.GetLivresAsync(disponible: true);
        ViewBag.Utilisateurs = await _api.GetUtilisateursAsync();
        return View(new CreateEmpruntViewModel { DateRetourPrevue = DateTime.Now.AddDays(14) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEmpruntViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Livres = await _api.GetLivresAsync(disponible: true);
            ViewBag.Utilisateurs = await _api.GetUtilisateursAsync();
            return View(vm);
        }
        var result = await _api.CreateEmpruntAsync(vm);
        if (result == null)
        {
            ModelState.AddModelError("", "Erreur lors de la création de l'emprunt.");
            ViewBag.Livres = await _api.GetLivresAsync(disponible: true);
            ViewBag.Utilisateurs = await _api.GetUtilisateursAsync();
            return View(vm);
        }
        TempData["Success"] = "Emprunt enregistré avec succès !";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Retour(int id)
    {
        await _api.RetournerLivreAsync(id);
        TempData["Success"] = "Retour enregistré avec succès !";
        return RedirectToAction(nameof(Index));
    }
}

public class ArticlesController : Controller
{
    private readonly ApiService _api;

    public ArticlesController(ApiService api) => _api = api;

    public async Task<IActionResult> Index(string? recherche, string? domaine, int page = 1)
    {
        var articles = await _api.GetArticlesAsync(recherche, domaine, page);
        var domaines = await _api.GetDomainesAsync();
        ViewBag.Domaines = domaines;
        ViewBag.Recherche = recherche;
        ViewBag.DomaineSelectionne = domaine;
        ViewBag.Page = page;
        return View(articles);
    }

    public async Task<IActionResult> Details(int id)
    {
        var article = await _api.GetArticleAsync(id);
        if (article == null) return NotFound();
        return View(article);
    }

    public IActionResult Create() => View(new CreateArticleViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateArticleViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var result = await _api.CreateArticleAsync(vm);
        if (result == null) { ModelState.AddModelError("", "Erreur lors de la création."); return View(vm); }
        TempData["Success"] = "Article ajouté avec succès !";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var a = await _api.GetArticleAsync(id);
        if (a == null) return NotFound();
        var vm = new CreateArticleViewModel
        {
            Titre = a.Titre, Auteur = a.Auteur, Revue = a.Revue,
            AnneePublication = a.AnneePublication, Domaine = a.Domaine,
            DOI = a.DOI, Resume = a.Resume, MotsCles = a.MotsCles, LienAcces = a.LienAcces
        };
        ViewBag.ArticleId = id;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateArticleViewModel vm)
    {
        if (!ModelState.IsValid) { ViewBag.ArticleId = id; return View(vm); }
        var result = await _api.UpdateArticleAsync(id, vm);
        if (result == null) { ModelState.AddModelError("", "Erreur."); ViewBag.ArticleId = id; return View(vm); }
        TempData["Success"] = "Article modifié avec succès !";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteArticleAsync(id);
        TempData["Success"] = "Article supprimé avec succès !";
        return RedirectToAction(nameof(Index));
    }
}

public class RechercheController : Controller
{
    private readonly ApiService _api;

    public RechercheController(ApiService api) => _api = api;

    public async Task<IActionResult> Index(string? q)
    {
        if (string.IsNullOrWhiteSpace(q)) return View(new RechercheResultatViewModel());
        var result = await _api.RechercherAsync(q);
        if (result != null) result.TermeRecherche = q;
        return View(result ?? new RechercheResultatViewModel { TermeRecherche = q });
    }
}
