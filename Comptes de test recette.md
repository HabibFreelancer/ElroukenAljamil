# Comptes de test — Phase de recette

Créés automatiquement au démarrage de l'Identity API via `TestAccountSeeder`.

## Compte Admin Recette

| Champ      | Valeur                        |
|------------|-------------------------------|
| Email      | recette.admin@elrouken.local  |
| Mot de passe | Recette@Admin1234           |
| Rôle       | Admin                         |
| ID         | A1000000-0000-0000-0000-000000000001 |

Accès : tableau de bord complet, changement de statut, stats, tous les tickets.

---

## Comptes Testeurs

| Email                      | Mot de passe  | Rôle   | ID |
|----------------------------|---------------|--------|----|
| testeur1@elrouken.local    | Testeur@1234  | Tester | B2000000-0000-0000-0000-000000000001 |
| testeur2@elrouken.local    | Testeur@1234  | Tester | B2000000-0000-0000-0000-000000000002 |
| testeur3@elrouken.local    | Testeur@1234  | Tester | B2000000-0000-0000-0000-000000000003 |

Accès : créer des tickets, voir ses propres tickets, valider/rejeter les correctifs (Résolu / KO).

---

## Compte Admin principal (existant)

| Email                   | Mot de passe  | Rôle  |
|-------------------------|---------------|-------|
| admin@elrouken.local    | Admin@1234    | Admin |

---

## Connexion

```
POST http://localhost:5000/api/auth/login
{
  "email": "testeur1@elrouken.local",
  "password": "Testeur@1234"
}
```

Le token JWT retourné contient le claim `role: Tester` (ou `Admin`).
