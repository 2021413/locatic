# Locatic — Agence de location de voitures

Application **ASP.NET Core MVC** (.NET 9) adossée à une base **SQLite** via **Entity Framework Core**.
Elle permet de gérer le parc de voitures d'une agence de location, ses clients et leurs réservations.

## Lancer le projet

```bash
# Depuis la racine du dépôt
dotnet run --project src/Locatic.Web
```

Au démarrage, l'application applique automatiquement les migrations EF Core
(`context.Database.Migrate()`) : la base `locatic.db` est créée si besoin et alimentée
avec des données de départ (seed). Les données **persistent** entre deux exécutions.

L'application est ensuite accessible sur l'URL indiquée dans la console (par défaut `https://localhost:5xxx`).
