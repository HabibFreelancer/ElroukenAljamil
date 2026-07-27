-- ============================================================
-- Seed : Matériel professionnel > Tracteurs + Workflow dépôt
-- Idempotent : vérifie l'existence avant chaque INSERT
-- ============================================================

DECLARE @menuId       INT;
DECLARE @parentCatId  INT;
DECLARE @tracteurCatId INT;
DECLARE @workflowId   INT;
DECLARE @stepDetailsId INT;

-- ── 1. Menu ──────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM Menus WHERE Slug = 'materiel-professionnel')
BEGIN
    INSERT INTO Menus (Name, Slug, Icon, DisplayOrder, IsActive,
                       CreatedAt, Version)
    VALUES (N'Matériel professionnel', 'materiel-professionnel', N'🚜', 10, 1,
            GETUTCDATE(), 0);
END
SELECT @menuId = Id FROM Menus WHERE Slug = 'materiel-professionnel';

-- ── 2. Catégorie parente ─────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM Categories
               WHERE Slug = 'materiel-professionnel' AND ParentCategoryId IS NULL)
BEGIN
    INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug,
                             ShowInDeposit, IsLink, DisplayOrder, IsActive,
                             CreatedAt, Version)
    VALUES (@menuId, NULL, N'Matériel professionnel', 'materiel-professionnel',
            0, 0, 1, 1, GETUTCDATE(), 0);
END
SELECT @parentCatId = Id FROM Categories
WHERE Slug = 'materiel-professionnel' AND ParentCategoryId IS NULL;

-- ── 3. Sous-catégorie Tracteurs ──────────────────────────────
IF NOT EXISTS (SELECT 1 FROM Categories
               WHERE Slug = 'tracteurs' AND ParentCategoryId = @parentCatId)
BEGIN
    INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug,
                             ShowInDeposit, IsLink, DisplayOrder, IsActive,
                             CreatedAt, Version)
    VALUES (@menuId, @parentCatId, N'Tracteurs', 'tracteurs',
            1, 1, 1, 1, GETUTCDATE(), 0);
END
SELECT @tracteurCatId = Id FROM Categories
WHERE Slug = 'tracteurs' AND ParentCategoryId = @parentCatId;

-- ── 4. Workflow ──────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM DepositWorkflows WHERE CategoryId = @tracteurCatId)
BEGIN
    INSERT INTO DepositWorkflows (CategoryId, Name, Description, IsActive,
                                  CreatedAt, Version)
    VALUES (@tracteurCatId, N'Dépôt Tracteur',
            N'Workflow de dépôt pour la catégorie Tracteurs', 1,
            GETUTCDATE(), 0);
END
SELECT @workflowId = Id FROM DepositWorkflows WHERE CategoryId = @tracteurCatId;

-- ── 5. Étapes ────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM WorkflowSteps WHERE WorkflowId = @workflowId)
BEGIN
    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey,
                                IsRequired, IsActive, CreatedAt)
    VALUES
    (@workflowId, 1, N'Dites-nous en plus',
     N'Renseignez les caractéristiques de votre tracteur', 'details', 1, 1, GETUTCDATE()),
    (@workflowId, 2, N'Décrivez votre bien !',
     N'Ajoutez un titre et une description détaillée', 'description', 1, 1, GETUTCDATE()),
    (@workflowId, 3, N'Quel est votre prix ?',
     N'Indiquez le prix de vente', 'price', 1, 1, GETUTCDATE()),
    (@workflowId, 4, N'Ajoutez des photos',
     N'Ajoutez jusqu''à 10 photos de votre tracteur', 'photos', 1, 1, GETUTCDATE()),
    (@workflowId, 5, N'Où se situe votre bien ?',
     N'Indiquez la localisation de votre tracteur', 'location', 1, 1, GETUTCDATE()),
    (@workflowId, 6, N'Vos coordonnées',
     N'Comment les acheteurs peuvent-ils vous contacter ?', 'contact', 1, 1, GETUTCDATE());
END

SELECT @stepDetailsId = Id FROM WorkflowSteps
WHERE WorkflowId = @workflowId AND StepKey = 'details';

-- ── 6. Champs de l'étape "Dites-nous en plus" ────────────────
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @stepDetailsId)
BEGIN
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder,
                             Options, DefaultValue, Suffix, HelperText,
                             IsRequired, DisplayOrder, IsActive,
                             MaxLength, ValidationRegex, VisibilityCondition,
                             CreatedAt)
    VALUES
    -- Année modèle
    (@stepDetailsId, 'annee_modele', N'Année modèle', 'number', N'Ex : 2018',
     '[]', '', '', '', 1, 1, 1, NULL, N'^\d{4}$', '', GETUTCDATE()),

    -- Puissance (CV)
    (@stepDetailsId, 'puissance', N'Puissance', 'number', N'Ex : 120',
     '[]', '', 'CV', '', 1, 2, 1, NULL, '', '', GETUTCDATE()),

    -- Marque (select)
    (@stepDetailsId, 'marque', N'Marque', 'select', N'Sélectionnez une marque',
     N'[{"value":"lamborghini","label":"Lamborghini"},{"value":"landini","label":"Landini"},{"value":"massey-ferguson","label":"Massey Ferguson"},{"value":"mc-cormick","label":"Mc Cormick"},{"value":"new-holland","label":"New Holland"},{"value":"renault","label":"Renault"}]',
     'landini', '', '', 1, 3, 1, NULL, '', '', GETUTCDATE()),

    -- Heures (H)
    (@stepDetailsId, 'heures', N'Heures', 'number', N'Ex : 3500',
     '[]', '', 'H', '', 0, 4, 1, NULL, '', '', GETUTCDATE());
END

PRINT N'Seed Tracteurs terminé. CategoryId=' + CAST(@tracteurCatId AS NVARCHAR)
    + N', WorkflowId=' + CAST(@workflowId AS NVARCHAR);
