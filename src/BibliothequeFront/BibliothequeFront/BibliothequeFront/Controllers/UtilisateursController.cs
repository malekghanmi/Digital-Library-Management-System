using Microsoft.AspNetCore.Mvc;
using BibliothequeFront.Models;
using BibliothequeFront.Services;

namespace BibliothequeFront.Controllers;

public class UtilisateursController : Controller
{
    private readonly ApiService _api;

    public UtilisateursController(ApiService api) => _api = api;

    public async Task<IActionResult> Index(string? recherche, int page = 1)
    {
        var utilisateurs = await _api.GetUtilisateursAsync(recherche, page);
        ViewBag.Recherche = recherche;
        ViewBag.Page = page;
        return View(utilisateurs);
    }

    public async Task<IActionResult> Details(int id)
    {
        var utilisateur = await _api.GetUtilisateurAsync(id);
        if (utilisateur == null) return NotFound();
        var emprunts = await _api.GetEmpruntsAsync(utilisateurId: id);
        ViewBag.Emprunts = emprunts;
        return View(utilisateur);
    }

    public IActionResult Create() => View(new CreateUtilisateurViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUtilisateurViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var result = await _api.CreateUtilisateurAsync(vm);
        if (result == null)
        {
            ModelState.AddModelError("", "Erreur lors de la création de l'utilisateur.");
            return View(vm);
        }
        TempData["Success"] = "Utilisateur créé avec succès !";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var u = await _api.GetUtilisateurAsync(id);
        if (u == null) return NotFound();
        var vm = new CreateUtilisateurViewModel
        {
            Nom = u.Nom, Prenom = u.Prenom, Email = u.Email,
            Telephone = u.Telephone, Adresse = u.Adresse,
            DateNaissance = u.DateNaissance, Role = u.Role
        };
        ViewBag.UtilisateurId = id;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateUtilisateurViewModel vm)
    {
        if (!ModelState.IsValid) { ViewBag.UtilisateurId = id; return View(vm); }
        var result = await _api.UpdateUtilisateurAsync(id, vm);
        if (result == null)
        {
            ModelState.AddModelError("", "Erreur lors de la mise à jour.");
            ViewBag.UtilisateurId = id;
            return View(vm);
        }
        TempData["Success"] = "Utilisateur modifié avec succès !";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteUtilisateurAsync(id);
        TempData["Success"] = "Utilisateur supprimé avec succès !";
        return RedirectToAction(nameof(Index));
    }
}
