# 📚 Bibliothèque Numérique — Guide de Démarrage Complet

## Architecture des deux projets

```
BibliothequeAPI  (Port 7200)    ←→    BibliothequeFront (Port 7100)
  └── Base de données SQL Server           └── Interface utilisateur MVC
  └── API REST (Swagger)                   └── Appels HTTP vers l'API
  └── Entity Framework Core (Code First)
```

---

## 🗄️ PROJET 1 : BibliothequeAPI (Base de données + API REST)

### Prérequis
- .NET 8 SDK
- SQL Server (LocalDB inclus avec Visual Studio)
- Visual Studio 2022 ou VS Code

### Installation

**1. Ouvrir le projet**
```
Ouvrir : BibliothequeAPI.sln
```

**2. Restaurer les packages NuGet**
```
dotnet restore
```
Ou dans Visual Studio : clic droit sur la solution → Restore NuGet Packages

**3. Vérifier la connexion SQL Server dans `appsettings.json`**
```json
"ConnectionStrings": {
  "BibliothequeDB": "Server=(localdb)\\mssqllocaldb;Database=BibliothequeDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```
> Si vous utilisez SQL Server Express, remplacez par :
> `Server=.\\SQLEXPRESS;Database=BibliothequeDB;Trusted_Connection=True;TrustServerCertificate=True`

**4. Créer la base de données (Code First Migration)**

Dans Visual Studio — Package Manager Console :
```powershell
Add-Migration InitialCreate
Update-Database
```

Ou en ligne de commande dans le dossier BibliothequeAPI :
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**5. Démarrer l'API**
```
dotnet run
```
Ou F5 dans Visual Studio.

L'API sera disponible sur :
- **HTTPS : https://localhost:7200**
- **HTTP  : http://localhost:5200**
- **Swagger UI : https://localhost:7200/swagger**

---

## 🖥️ PROJET 2 : BibliothequeFront (Interface MVC)

### Installation

**1. Ouvrir le projet**
```
Ouvrir : BibliothequeFront.sln
```

**2. Restaurer les packages**
```
dotnet restore
```

**3. Configurer l'URL de l'API dans `appsettings.json`**
```json
"ApiSettings": {
  "BaseUrl": "https://localhost:7200/"
}
```
> Cette URL doit correspondre exactement au port où tourne BibliothequeAPI.

**4. Démarrer le Front**
```
dotnet run
```

Le Front sera disponible sur :
- **HTTPS : https://localhost:7100**
- **HTTP  : http://localhost:5100**

---

## 🔗 CONNEXION ENTRE LES DEUX PROJETS

### ÉTAPE CRITIQUE — Certificat HTTPS en développement
Si vous avez des erreurs SSL entre les deux projets, exécutez :
```bash
dotnet dev-certs https --trust
```

### CORS — Déjà configuré dans BibliothequeAPI
Le fichier `Program.cs` de l'API autorise déjà le Front :
```csharp
policy.WithOrigins(
    "https://localhost:7100",
    "http://localhost:5100"
)
```
> Si votre Front tourne sur un port différent, modifiez cette liste dans `BibliothequeAPI/Program.cs`.

### Ordre de démarrage
```
1. Démarrer BibliothequeAPI EN PREMIER
2. Attendre que l'API soit prête (message "Now listening on https://localhost:7200")
3. Démarrer BibliothequeFront
4. Naviguer vers https://localhost:7100
```

### Démarrage simultané (Visual Studio)
- Clic droit sur la Solution → Properties
- Startup Project → Multiple startup projects
- BibliothequeAPI : Start
- BibliothequeFront : Start
- OK

---

## ✅ Vérification que tout fonctionne

1. **Tester l'API** : Ouvrir https://localhost:7200/swagger → Tester GET /api/livres → Réponse 200 avec les livres de seed
2. **Tester le Front** : Ouvrir https://localhost:7100 → Le tableau de bord doit afficher les statistiques
3. **Si les stats sont à 0 ou erreur** : Vérifier que l'API est bien démarrée et que le CORS est configuré

---

## 📊 Fonctionnalités disponibles

| Module | Fonctionnalités |
|--------|----------------|
| **Livres** | CRUD complet, recherche, filtre genre/disponibilité, pagination |
| **Utilisateurs** | CRUD, rôles (Admin/Bibliothécaire/Membre), historique emprunts |
| **Emprunts** | Création, retour, calcul pénalités automatique, filtre statut |
| **Articles** | CRUD, recherche par domaine/mots-clés, lien DOI |
| **Recherche** | Recherche globale livres + articles simultanément |
| **Statistiques** | Dashboard avec compteurs animés en temps réel |

---

## 🐛 Problèmes courants

### "Unable to connect to the remote server"
→ L'API n'est pas démarrée. Démarrer BibliothequeAPI en premier.

### "SSL Certificate error" / ERR_CERT_INVALID
→ Exécuter : `dotnet dev-certs https --trust`

### "Migration already exists"
→ Supprimer le dossier `Migrations/` et recommencer : `dotnet ef migrations add InitialCreate`

### Le tableau de bord affiche "Impossible de contacter l'API"
→ Vérifier l'URL dans `BibliothequeFront/appsettings.json` → `ApiSettings:BaseUrl`
→ Doit être exactement `https://localhost:7200/`

### Erreur CORS dans la console du navigateur
→ Vérifier dans `BibliothequeAPI/Program.cs` que le port du Front est bien dans la liste `WithOrigins`

---

## 🏗️ Structure des projets

```
BibliothequeAPI/
├── Controllers/
│   ├── LivresController.cs
│   ├── UtilisateursController.cs
│   ├── EmpruntsController.cs
│   ├── ArticlesController.cs
│   └── StatsRechercheController.cs
├── Models/
│   ├── Livre.cs
│   ├── Utilisateur.cs
│   ├── Emprunt.cs
│   └── Article.cs
├── Data/
│   └── BibliothequeContext.cs   (Code First + Seed)
├── DTOs/
│   └── BibliothequeDto.cs
├── Program.cs                   (CORS + Swagger + Auto-Migration)
└── appsettings.json             (Connexion SQL Server — PORT 7200)

BibliothequeFront/
├── Controllers/
│   ├── HomeController.cs
│   ├── LivresController.cs
│   ├── UtilisateursController.cs
│   └── OtherControllers.cs      (Emprunts, Articles, Recherche)
├── Models/
│   └── ViewModels.cs
├── Services/
│   └── ApiService.cs            (Client HTTP vers l'API)
├── Views/
│   ├── Home/Index.cshtml        (Dashboard avec stats 3D)
│   ├── Livres/                  (Index, Details, Create, Edit)
│   ├── Utilisateurs/            (Index, Details, Create, Edit)
│   ├── Emprunts/                (Index, Create)
│   ├── Articles/                (Index, Details, Create, Edit)
│   ├── Recherche/               (Index)
│   └── Shared/_Layout.cshtml   (Navigation + Footer)
├── wwwroot/
│   ├── css/bibliotheque.css     (Design 3D luxe — Or/Bordeaux/Ivoire)
│   └── js/bibliotheque.js       (Effets 3D tilt, compteurs animés)
├── Program.cs                   (HttpClient → API)
└── appsettings.json             (URL API — PORT 7100)
```
