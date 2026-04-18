using Microsoft.AspNetCore.Mvc;
using BibliothequeFront.Services;

namespace BibliothequeFront.Controllers;

public class HomeController : Controller
{
    private readonly ApiService _api;

    public HomeController(ApiService api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _api.GetStatistiquesAsync();
        return View(stats);
    }
}
