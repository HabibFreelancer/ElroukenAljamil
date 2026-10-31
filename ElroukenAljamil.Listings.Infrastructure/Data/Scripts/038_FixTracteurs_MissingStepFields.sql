-- ============================================================
-- Fix : Tracteurs — Champs manquants étape "description"
-- Idempotent
-- ============================================================

DECLARE @tracteurCatId INT;
DECLARE @workflowId    INT;
DECLARE @stepDescId    INT;
DECLARE @stepPriceId   INT;
DECLARE @stepLocationId INT;
DECLARE @stepContactId INT;

SELECT @tracteurCatId = c.Id
FROM Categories c
JOIN Categories p ON c.ParentCategoryId = p.Id
WHERE c.Slug = 'tracteurs' AND p.Slug = 'materiel-professionnel';

SELECT @workflowId     = Id FROM DepositWorkflows WHERE CategoryId = @tracteurCatId;
SELECT @stepDescId     = Id FROM WorkflowSteps WHERE WorkflowId = @workflowId AND StepKey = 'description';
SELECT @stepPriceId    = Id FROM WorkflowSteps WHERE WorkflowId = @workflowId AND StepKey = 'price';
SELECT @stepLocationId = Id FROM WorkflowSteps WHERE WorkflowId = @workflowId AND StepKey = 'location';
SELECT @stepContactId  = Id FROM WorkflowSteps WHERE WorkflowId = @workflowId AND StepKey = 'contact';

-- ── 1. Étape description : titre + description ───────────────
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @stepDescId AND FieldKey = 'description')
BEGIN
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder,
                             Options, DefaultValue, Suffix, HelperText,
                             IsRequired, DisplayOrder, IsActive,
                             MaxLength, ValidationRegex, VisibilityCondition, CreatedAt)
    VALUES
    (@stepDescId, 'description', N'Titre de l''annonce', 'text_counter',
     N'Ex: Tracteur Massey Ferguson 120CV - 2018 - 3500h',
     '', '', '', '', 1, 1, 1, 200, '', '', GETUTCDATE()),

    (@stepDescId, 'annonce_description', N'Description de l''annonce', 'textarea_counter',
     N'Décrivez votre tracteur : état général, entretien, équipements, historique...',
     '', '', '', '', 1, 2, 1, 4000, '', '', GETUTCDATE());
END

-- ── 2. Étape price : champ prix ──────────────────────────────
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @stepPriceId)
BEGIN
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder,
                             Options, DefaultValue, Suffix, HelperText,
                             IsRequired, DisplayOrder, IsActive,
                             MaxLength, ValidationRegex, VisibilityCondition, CreatedAt)
    VALUES
    (@stepPriceId, 'price', N'Prix de vente', 'price_gauge',
     N'Ex: 45000', '', '', 'TND', '',
     1, 1, 1, NULL, '', '', GETUTCDATE());
END

-- ── 3. Étape location : adresse ──────────────────────────────
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @stepLocationId)
BEGIN
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder,
                             Options, DefaultValue, Suffix, HelperText,
                             IsRequired, DisplayOrder, IsActive,
                             MaxLength, ValidationRegex, VisibilityCondition, CreatedAt)
    VALUES
    (@stepLocationId, 'address', N'Localisation', 'address',
     N'Tapez votre adresse...', '', '', '',
     N'Indiquez où se trouve le tracteur.',
     1, 1, 1, NULL, '', '', GETUTCDATE());
END

-- ── 4. Étape contact : email + téléphone + masquer ───────────
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @stepContactId)
BEGIN
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder,
                             Options, DefaultValue, Suffix, HelperText,
                             IsRequired, DisplayOrder, IsActive,
                             MaxLength, ValidationRegex, VisibilityCondition, CreatedAt)
    VALUES
    (@stepContactId, 'email',     N'Email',            'email',  '', '', '', '', '', 1, 1, 1, NULL, '', '', GETUTCDATE()),
    (@stepContactId, 'phone',     N'Téléphone',         'phone',  '', '', '', '', '', 1, 2, 1, NULL, '', '', GETUTCDATE()),
    (@stepContactId, 'hidePhone', N'Masquer le numéro', 'toggle', '', '', 'false', '', '', 0, 3, 1, NULL, '', '', GETUTCDATE());
END

PRINT N'Fix champs Tracteurs terminé.';
