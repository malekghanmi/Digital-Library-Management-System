using System.Text;
using System.Text.Json;
using BibliothequeFront.Models;

namespace BibliothequeFront.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient http)
    {
        _http = http;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    // ===== LIVRES =====
    public async Task<List<LivreViewModel>> GetLivresAsync(string? recherche = null, string? genre = null, bool? disponible = null, int page = 1)
    {
        var url = $"api/livres?page={page}&pageSize=12";
        if (!string.IsNullOrWhiteSpace(recherche)) url += $"&recherche={Uri.EscapeDataString(recherche)}";
        if (!string.IsNullOrWhiteSpace(genre)) url += $"&genre={Uri.EscapeDataString(genre)}";
        if (disponible.HasValue) url += $"&disponible={disponible}";
        return await GetAsync<List<LivreViewModel>>(url) ?? new();
    }

    public async Task<LivreViewModel?> GetLivreAsync(int id) =>
        await GetAsync<LivreViewModel>($"api/livres/{id}");

    public async Task<LivreViewModel?> CreateLivreAsync(CreateLivreViewModel vm) =>
        await PostAsync<LivreViewModel>("api/livres", vm);

    public async Task<LivreViewModel?> UpdateLivreAsync(int id, CreateLivreViewModel vm) =>
        await PutAsync<LivreViewModel>($"api/livres/{id}", vm);

    public async Task<bool> DeleteLivreAsync(int id) =>
        await DeleteAsync($"api/livres/{id}");

    public async Task<List<string>> GetGenresAsync() =>
        await GetAsync<List<string>>("api/livres/genres") ?? new();

    // ===== UTILISATEURS =====
    public async Task<List<UtilisateurViewModel>> GetUtilisateursAsync(string? recherche = null, int page = 1)
    {
        var url = $"api/utilisateurs?page={page}&pageSize=12";
        if (!string.IsNullOrWhiteSpace(recherche)) url += $"&recherche={Uri.EscapeDataString(recherche)}";
        return await GetAsync<List<UtilisateurViewModel>>(url) ?? new();
    }

    public async Task<UtilisateurViewModel?> GetUtilisateurAsync(int id) =>
        await GetAsync<UtilisateurViewModel>($"api/utilisateurs/{id}");

    public async Task<UtilisateurViewModel?> CreateUtilisateurAsync(CreateUtilisateurViewModel vm) =>
        await PostAsync<UtilisateurViewModel>("api/utilisateurs", vm);

    public async Task<UtilisateurViewModel?> UpdateUtilisateurAsync(int id, CreateUtilisateurViewModel vm) =>
        await PutAsync<UtilisateurViewModel>($"api/utilisateurs/{id}", vm);

    public async Task<bool> DeleteUtilisateurAsync(int id) =>
        await DeleteAsync($"api/utilisateurs/{id}");

    // ===== EMPRUNTS =====
    public async Task<List<EmpruntViewModel>> GetEmpruntsAsync(string? statut = null, int page = 1, int? utilisateurId = null)
    {
        var url = $"api/emprunts?page={page}&pageSize=12";
        if (!string.IsNullOrWhiteSpace(statut)) url += $"&statut={statut}";
        if (utilisateurId.HasValue) url += $"&utilisateurId={utilisateurId}";
        return await GetAsync<List<EmpruntViewModel>>(url) ?? new();
    }

    public async Task<EmpruntViewModel?> CreateEmpruntAsync(CreateEmpruntViewModel vm) =>
        await PostAsync<EmpruntViewModel>("api/emprunts", vm);

    public async Task<EmpruntViewModel?> RetournerLivreAsync(int empruntId) =>
        await PutAsync<EmpruntViewModel>("api/emprunts/retour", new { EmpruntId = empruntId, DateRetourReelle = DateTime.UtcNow });

    // ===== ARTICLES =====
    public async Task<List<ArticleViewModel>> GetArticlesAsync(string? recherche = null, string? domaine = null, int page = 1)
    {
        var url = $"api/articles?page={page}&pageSize=12";
        if (!string.IsNullOrWhiteSpace(recherche)) url += $"&recherche={Uri.EscapeDataString(recherche)}";
        if (!string.IsNullOrWhiteSpace(domaine)) url += $"&domaine={Uri.EscapeDataString(domaine)}";
        return await GetAsync<List<ArticleViewModel>>(url) ?? new();
    }

    public async Task<ArticleViewModel?> GetArticleAsync(int id) =>
        await GetAsync<ArticleViewModel>($"api/articles/{id}");

    public async Task<ArticleViewModel?> CreateArticleAsync(CreateArticleViewModel vm) =>
        await PostAsync<ArticleViewModel>("api/articles", vm);

    public async Task<ArticleViewModel?> UpdateArticleAsync(int id, CreateArticleViewModel vm) =>
        await PutAsync<ArticleViewModel>($"api/articles/{id}", vm);

    public async Task<bool> DeleteArticleAsync(int id) =>
        await DeleteAsync($"api/articles/{id}");

    public async Task<List<string>> GetDomainesAsync() =>
        await GetAsync<List<string>>("api/articles/domaines") ?? new();

    // ===== STATISTIQUES =====
    public async Task<StatistiquesViewModel?> GetStatistiquesAsync() =>
        await GetAsync<StatistiquesViewModel>("api/statistiques");

    // ===== RECHERCHE =====
    public async Task<RechercheResultatViewModel?> RechercherAsync(string q) =>
        await GetAsync<RechercheResultatViewModel>($"api/recherche?q={Uri.EscapeDataString(q)}");

    // ===== HELPERS HTTP =====
    private async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode) return default;
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        catch { return default; }
    }

    private async Task<T?> PostAsync<T>(string url, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync(url, content);
            if (!response.IsSuccessStatusCode) return default;
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
        }
        catch { return default; }
    }

    private async Task<T?> PutAsync<T>(string url, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PutAsync(url, content);
            if (!response.IsSuccessStatusCode) return default;
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
        }
        catch { return default; }
    }

    private async Task<bool> DeleteAsync(string url)
    {
        try
        {
            var response = await _http.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}