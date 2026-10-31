-- ============================================================
-- 040_FixMaterielProfessionnel.sql
-- 1. Icône du menu Matériel professionnel → fa-solid fa-industry
-- 2. Position du menu après Véhicules (DisplayOrder=3)
-- 3. Ajouter "Autre" à la fin des options du champ "marque" (Tracteurs)
-- ============================================================

-- ── 1. Corriger l'icône du menu ──────────────────────────────
UPDATE Menus
SET Icon = 'fa-solid fa-industry'
WHERE Slug = 'materiel-professionnel';

-- ── 2. Repositionner le menu après Véhicules ─────────────────
-- Véhicules est à DisplayOrder=2, on veut Matériel professionnel à 3
-- D'abord décaler tous les menus qui sont à DisplayOrder >= 3 (sauf Matériel pro)
UPDATE Menus
SET DisplayOrder = DisplayOrder + 1
WHERE DisplayOrder >= 3
  AND Slug != 'materiel-professionnel';

-- Puis placer Matériel professionnel à 3
UPDATE Menus
SET DisplayOrder = 3
WHERE Slug = 'materiel-professionnel';

-- ── 3. Ajouter "Autre" dans les options du champ "marque" ────
-- Le champ marque du workflow Tracteurs stocke ses options en JSON
-- On ajoute {"value":"autre","label":"Autre"} à la fin du tableau JSON

DECLARE @stepDetailsId INT = (
    SELECT sf.StepId
    FROM StepFields sf
    INNER JOIN WorkflowSteps ws ON sf.StepId = ws.Id
    INNER JOIN DepositWorkflows w ON ws.WorkflowId = w.Id
    INNER JOIN Categories c ON w.CategoryId = c.Id
    WHERE sf.FieldKey = 'marque'
      AND c.Slug = 'tracteurs'
);

IF @stepDetailsId IS NOT NULL
BEGIN
    UPDATE StepFields
    SET Options = JSON_MODIFY(
        Options,
        'append $',
        JSON_QUERY('{"value":"autre","label":"Autre"}')
    )
    WHERE StepId = @stepDetailsId
      AND FieldKey = 'marque'
      AND Options NOT LIKE '%"value":"autre"%';
END

PRINT N'Script 040 terminé.';
