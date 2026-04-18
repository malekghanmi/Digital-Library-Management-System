using Microsoft.AspNetCore.Mvc;
using BibliothequeFront.Models;
using BibliothequeFront.Services;

namespace BibliothequeFront.Controllers;

public class LivresController : Controller
{
    private readonly ApiService _api;

    public LivresController(ApiService api) => _api = api;

    public async Task<IActionResult> Index(string? recherche, string? genre, bool? disponible, int page = 1)
    {
        var livres = await _api.GetLivresAsync(recherche, genre, disponible, page);
        var genres = await _api.GetGenresAsync();
        ViewBag.Genres = genres;
        ViewBag.Recherche = recherche;
        ViewBag.GenreSelectionne = genre;
        ViewBag.Page = page;
        return View(livres);
    }

    public async Task<IActionResult> Details(int id)
    {
        var livre = await _api.GetLivreAsync(id);
        if (livre == null) return NotFound();
        return View(livre);
    }

    public IActionResult Create() => View(new CreateLivreViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLivreViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var result = await _api.CreateLivreAsync(vm);
        if (result == null)
        {
            ModelState.AddModelError("", "Erreur lors de la création du livre.");
            return View(vm);
        }
        TempData["Success"] = "Livre ajouté avec succès !";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var livre = await _api.GetLivreAsync(id);
        if (livre == null) return NotFound();
        var vm = new CreateLivreViewModel
        {
            Titre = livre.Titre, Auteur = livre.Auteur, ISBN = livre.ISBN,
            Editeur = livre.Editeur, AnneePublication = livre.AnneePublication,
            Genre = livre.Genre, Categorie = livre.Categorie,
            NombreExemplaires = livre.NombreExemplaires,
            Description = livre.Description, ImageCouverture = livre.ImageCouverture
        };
        ViewBag.LivreId = id;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateLivreViewModel vm)
    {
        if (!ModelState.IsValid) { ViewBag.LivreId = id; return View(vm); }
        var result = await _api.UpdateLivreAsync(id, vm);
        if (result == null)
        {
            ModelState.AddModelError("", "Erreur lors de la mise à jour.");
            ViewBag.LivreId = id;
            return View(vm);
        }
        TempData["Success"] = "Livre modifié avec succès !";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteLivreAsync(id);
        TempData["Success"] = "Livre supprimé avec succès !";
        return RedirectToAction(nameof(Index));
    }
}
