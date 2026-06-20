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

## Architecture en couches

Le code est découpé en quatre projets, des dépendances allant toujours vers le domaine
(le domaine ne dépend de personne) :

| Projet | Responsabilité | Dépend de |
|--------|----------------|-----------|
| `Locatic.Domain` | Entités métier (POCO) et énumérations. Aucune dépendance technique. | — |
| `Locatic.Application` | Contrats (`IRepository`, repositories, services), **logique métier** (services), `OperationResult`. | Domain |
| `Locatic.Infrastructure` | `DbContext` EF Core, configurations, migrations, **implémentations des repositories**, seed, enregistrement DI. | Domain, Application |
| `Locatic.Web` | Controllers (fins), vues Razor, ViewModels, `Program.cs`. | Application, Infrastructure |

Principes appliqués :

- **Injection de dépendances** : le `DbContext`, les repositories et les services sont
  enregistrés dans `Infrastructure/DependencyInjection.cs` (`AddLocatic`) et injectés par constructeur.
- **Inversion de dépendance (SOLID)** : les services dépendent des interfaces `IRepository`
  (couche Application), pas d'EF Core. Les implémentations vivent dans l'Infrastructure.
- **Controllers fins** : ils orchestrent (mapping ViewModel ↔ entité, appel de service, redirection).
  Aucun accès direct à la base ni règle métier dans les controllers.
- **ViewModels dédiés** pour les formulaires, avec **validation côté serveur** (DataAnnotations + `ModelState`).

## Modèle de données

```
Marque 1 ──< Modele 1 ──< Voiture 1 ──< Reservation >── 1 Client
```

- Une **Marque** regroupe plusieurs **Modèles** ; un Modèle appartient à une seule Marque.
- Un **Modèle** se décline en plusieurs **Voitures** ; une Voiture est d'un seul Modèle.
- Une **Réservation** relie **une** Voiture et **un** Client sur une période.

Les propriétés de navigation permettent la chaîne `Voiture → Modele → Marque`,
exploitée partout dans les vues (on affiche la marque et le modèle, jamais un simple identifiant).
