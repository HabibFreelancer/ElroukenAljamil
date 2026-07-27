-- ============================================
-- 035: Seed Deposit Workflow for "Immobilier > Bureau & Commerce"
-- Structure: same 7 steps as Ventes immobilières
-- Differences:
--   - details step: no propertyType pills, businessType select + specific fields
--   - price step: salePrice + taxFonciere + chargesCopro
-- ============================================

DECLARE @BureauCategoryId INT = (
    SELECT Id FROM Categories
    WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'immobilier')
      AND Name = N'Bureau & Commerce'
      AND ParentCategoryId IS NULL
);

IF @BureauCategoryId IS NULL
BEGIN
    PRINT 'ERROR: Category "Bureau & Commerce" not found.';
    RETURN;
END

PRINT 'Bureau & Commerce CategoryId = ' + CAST(@BureauCategoryId AS VARCHAR);

IF EXISTS (SELECT 1 FROM DepositWorkflows WHERE CategoryId = @BureauCategoryId)
BEGIN
    PRINT 'Workflow for Bureau & Commerce already exists. Skipping.';
    RETURN;
END

-- ============================================================
-- 1. Create the workflow
-- ============================================================
INSERT INTO DepositWorkflows (CategoryId, Name, Description, IsActive, CreatedAt)
VALUES (@BureauCategoryId,
        N'Depot Bureau et Commerce',
        N'Workflow pour deposer une annonce de local professionnel ou commercial',
        1, GETUTCDATE());

DECLARE @WorkflowId INT = SCOPE_IDENTITY();

-- ============================================================
-- 2. Create 7 steps
-- ============================================================
INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired, IsActive) VALUES
(@WorkflowId, 1, N'Commencons par l''essentiel !',
    N'* champs obligatoires', 'title', 1, 1),
(@WorkflowId, 2, N'Ou se situe votre bien ?',
    N'Indiquez la localisation de votre local.', 'location', 1, 1),
(@WorkflowId, 3, N'Dites-nous en plus',
    N'Renseignez les caracteristiques de votre local professionnel.', 'details', 1, 1),
(@WorkflowId, 4, N'Ajoutez des photos',
    N'Les annonces avec photos recoivent plus de contacts.', 'photos', 0, 1),
(@WorkflowId, 5, N'Decrivez votre bien !',
    N'Une bonne description augmente vos chances de conclure rapidement.', 'description', 1, 1),
(@WorkflowId, 6, N'Quel est votre prix ?',
    N'Indiquez le prix et les charges du bien.', 'price', 1, 1),
(@WorkflowId, 7, N'Vos coordonnees',
    N'Verifiez vos informations de contact avant publication.', 'contact', 1, 1);

DECLARE @StepTitle   INT, @StepLocation INT, @StepDetails INT;
DECLARE @StepPhotos  INT, @StepDesc     INT, @StepPrice   INT, @StepContact INT;

SELECT @StepTitle    = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'title';
SELECT @StepLocation = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'location';
SELECT @StepDetails  = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'details';
SELECT @StepPhotos   = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'photos';
SELECT @StepDesc     = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'description';
SELECT @StepPrice    = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'price';
SELECT @StepContact  = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'contact';

-- ============================================================
-- 3. Title step
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
    Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
VALUES (@StepTitle, 'title', N'Titre de l''annonce', 'text',
    N'Ex: Bureau 80m2 centre-ville Tunis - vue dégagée',
    '', '', '', '', 1, 1, 1, 200, '', '');

-- ============================================================
-- 4. Location step
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
    Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
VALUES (@StepLocation, 'address', N'Adresse', 'address', N'Tapez votre adresse...', '', '', '',
    N'Completez votre adresse pour etre trouve plus facilement par les professionnels.',
    1, 1, 1, NULL, '', '');

-- ============================================================
-- 5. Details step — bureau/commerce specific (no propertyType pills)
--    FieldKey 'businessType' acts as the category discriminator
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
    Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
VALUES

-- businessType: type d'activité (single select, always visible)
(@StepDetails, 'businessType', N'Type d''activité', 'select', N'Choisissez',
 N'[{"value":"bureaux","label":"Bureaux"},{"value":"conteneurs","label":"Conteneurs"},{"value":"entrepots","label":"Entrepôts"},{"value":"restaurants_hotels","label":"Restaurants & Hôtels"},{"value":"boutiques_kiosques","label":"Boutiques & Kiosques"},{"value":"autres","label":"Autres commerces"}]',
 '', '', '', 1, 1, 1, NULL, '', ''),

-- ── Caractéristiques ─────────────────────────────────────────────────────

-- surface: Surface habitable (toujours visible)
(@StepDetails, 'surface', N'Surface habitable', 'number', N'0',
 '', '', N'm²', N'Surface totale du local en m².', 1, 2, 1, NULL, '', ''),

-- divisibleSurface: Surface divisible minimale
(@StepDetails, 'divisibleSurface', N'Surface divisible minimale', 'number', N'0',
 '', '', N'm²',
 N'Indiquez la surface minimale si le local peut être divisé.',
 0, 3, 1, NULL, '', ''),

-- levels: Nombre d'étages (entrepôts, bureaux)
(@StepDetails, 'levels', N'Nombre d''étages', 'number', N'Ex: 2',
 '', '', N'étage(s)', N'Indiquez 1 pour un local de plain-pied.',
 0, 4, 1, NULL, '', ''),

-- floor: Étage du local
(@StepDetails, 'floor', N'Étage de votre bien', 'number', N'Ex: 3',
 '', '0', N'ème étage', N'Indiquez 0 pour un rez-de-chaussée.',
 0, 5, 1, NULL, '', ''),

-- elevator: Ascenseur
(@StepDetails, 'elevator', N'Ascenseur', 'toggle', '', '', 'false', '', '',
 0, 6, 1, NULL, '', ''),

-- exterior: Extérieur (terrasse, parking de façade...)
(@StepDetails, 'exterior', N'Extérieur', 'multiselect', '',
 N'[{"value":"terrasse","label":"Terrasse"},{"value":"jardin","label":"Jardin"},{"value":"vitrine","label":"Vitrine"},{"value":"autre","label":"Autre"}]',
 '', '', '', 0, 7, 1, NULL, '', ''),

-- parking: Places de parking
(@StepDetails, 'parking', N'Places de parking', 'select', N'Choisissez',
 N'[{"value":"0","label":"0"},{"value":"1","label":"1"},{"value":"2","label":"2"},{"value":"3","label":"3"},{"value":"4","label":"4"},{"value":"5","label":"5"},{"value":"6","label":"6 et plus"},{"value":"autre","label":"Autre"}]',
 '', '', '', 0, 8, 1, NULL, '', ''),

-- constructionYear: Année de construction
(@StepDetails, 'constructionYear', N'Année de construction', 'year', N'AAAA',
 '', '', '', '', 0, 9, 1, 4, '', ''),

-- availableFrom: Disponible à partir de
(@StepDetails, 'availableFrom', N'Disponible à partir de', 'date_month', N'MM/AAAA',
 '', '', '', '', 0, 10, 1, 7, '', '');

-- ============================================================
-- 6. Description step (AI enabled)
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
    Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
VALUES
(@StepDesc, 'description', N'Titre de l''annonce', 'text_counter',
 N'Ex: Bureau moderne 80m2 avec vue panoramique - Tunis centre',
 '', '', '', '', 1, 1, 1, 200, '', ''),
(@StepDesc, 'annonce_description', N'Description de l''annonce', 'textarea_counter',
 N'Decrivez votre local : surface, agencement, equipements, accessibilite, avantages...',
 '', '', '', 'ai_enabled', 1, 2, 1, 4000, '', '');

-- ============================================================
-- 7. Price step — BUREAU & COMMERCE SPECIFIC
--    salePrice + taxFonciere + chargesCopro
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
    Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
VALUES
-- Section: prix de vente
(@StepPrice, 'salePrice', N'Votre prix de vente', 'number', N'Ex: 350000',
 '', '', N'TND', '', 1, 1, 1, NULL, '', ''),

-- Divider section: "Charges du bien"
(@StepPrice, 'section_charges', N'Charges du bien', 'section_title',
 '', '', '', '', '', 0, 2, 1, NULL, '', ''),

-- taxFonciere: Taxe foncière
(@StepPrice, 'taxFonciere', N'Taxe foncière', 'number', N'Ex: 1200',
 '', '', N'TND / an', N'Montant annuel de la taxe foncière.',
 0, 3, 1, NULL, '', ''),

-- chargesCopro: Charges annuelles de copropriété
(@StepPrice, 'chargesCopro', N'Charges annuelles de copropriété', 'number', N'Ex: 2400',
 '', '', N'TND / an', N'Montant annuel des charges de copropriété.',
 0, 4, 1, NULL, '', '');

-- ============================================================
-- 8. Contact step
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
    Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
VALUES
(@StepContact, 'email',     N'Email',            'email',  '', '', '', '', '', 1, 1, 1, NULL, '', ''),
(@StepContact, 'phone',     N'Téléphone',         'phone',  '', '', '', '', '', 1, 2, 1, NULL, '', ''),
(@StepContact, 'hidePhone', N'Masquer le numéro', 'toggle', '', '', 'false', '', '', 0, 3, 1, NULL, '', '');

PRINT 'Script 035: Workflow "Bureau & Commerce" created successfully.';
