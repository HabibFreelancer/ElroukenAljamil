-- ============================================
-- 034: Seed Deposit Workflow for "Immobilier > Colocations"
-- Types: Maison, Appartement, Autre (3 icons only)
-- Colocation-specific fields: roomType, furnished, roommatesCount,
--   smokingPolicy, petsAllowed, heatingType
-- ============================================

DECLARE @ColocCategoryId INT = (
    SELECT Id FROM Categories
    WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'immobilier')
      AND Name = N'Colocations'
      AND ParentCategoryId IS NULL
);

IF @ColocCategoryId IS NULL
BEGIN
    PRINT 'ERROR: Category "Colocations" not found.';
    RETURN;
END

PRINT 'Colocations CategoryId = ' + CAST(@ColocCategoryId AS VARCHAR);

IF EXISTS (SELECT 1 FROM DepositWorkflows WHERE CategoryId = @ColocCategoryId)
BEGIN
    PRINT 'Workflow for Colocations already exists. Skipping.';
    RETURN;
END

-- ============================================================
-- 1. Create workflow
-- ============================================================
INSERT INTO DepositWorkflows (CategoryId, Name, Description, IsActive, CreatedAt)
VALUES (@ColocCategoryId,
        N'Depot Colocation',
        N'Workflow pour deposer une annonce de colocation',
        1, GETUTCDATE());

DECLARE @WorkflowId INT = SCOPE_IDENTITY();

-- ============================================================
-- 2. Create 7 steps
-- ============================================================
INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired, IsActive) VALUES
(@WorkflowId, 1, N'Commencons par l''essentiel !',
    N'* champs obligatoires', 'title', 1, 1),
(@WorkflowId, 2, N'Ou se situe votre bien ?',
    N'Indiquez la localisation de votre bien.', 'location', 1, 1),
(@WorkflowId, 3, N'Dites-nous en plus',
    N'Selectionnez le type de bien et renseignez les criteres de votre colocation.', 'details', 1, 1),
(@WorkflowId, 4, N'Ajoutez des photos',
    N'Les annonces avec photos recoivent plus de contacts.', 'photos', 0, 1),
(@WorkflowId, 5, N'Decrivez votre colocation !',
    N'Une bonne description augmente vos chances de trouver le bon colocataire.', 'description', 1, 1),
(@WorkflowId, 6, N'Quel est votre loyer ?',
    N'Indiquez le loyer et les conditions financieres.', 'price', 1, 1),
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
    N'Ex: Chambre privee dans colocation 3 pieces Tunis',
    '', '', '', '', 1, 1, 1, 200, '', '');

-- ============================================================
-- 4. Location step
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
    Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
VALUES (@StepLocation, 'address', N'Adresse', 'address',
    N'Tapez votre adresse...', '', '', '',
    N'Completez votre adresse pour etre trouve plus facilement.',
    1, 1, 1, NULL, '', '');

-- ============================================================
-- 5. Details step — colocation-specific
--    propertyType: Maison / Appartement / Autre (3 only)
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
    Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
VALUES

-- ── propertyType (3 options: maison, appartement, autre) ──────────────────
(@StepDetails, 'propertyType', N'Choisissez votre type de bien', 'pills', '',
 N'[{"value":"maison","label":"Maison"},{"value":"appartement","label":"Appartement"},{"value":"autre","label":"Autre"}]',
 '', '', '', 1, 1, 1, NULL, '', ''),

-- ── Caractéristiques ──────────────────────────────────────────────────────

-- roomType: Chambre privée / Chambre partagée — visible tous
(@StepDetails, 'roomType', N'Type', 'select', N'Choisissez',
 N'[{"value":"privee","label":"Chambre privée"},{"value":"partagee","label":"Chambre partagée"}]',
 '', '', '', 1, 2, 1, NULL, '',
 N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- furnished: Meublé / Non meublé — visible tous
(@StepDetails, 'furnished', N'Ce bien est :', 'radio', '',
 N'[{"value":"non_meuble","label":"Non meublé"},{"value":"meuble","label":"Meublé"}]',
 'non_meuble', '', '', 1, 3, 1, NULL, '',
 N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- roommatesCount: Nombre de colocataires — visible tous
(@StepDetails, 'roommatesCount', N'Nombre de colocataires', 'number', N'Ex: 3',
 '', '', N'personne(s)', '', 0, 4, 1, NULL, '',
 N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- surface: Surface habitable — visible tous
(@StepDetails, 'surface', N'Surface habitable', 'number', N'0',
 '', '', N'm²',
 N'Comptez les surfaces interieures habitables d''une hauteur sous plafond de plus de 1,80m.',
 1, 5, 1, NULL, '',
 N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- rooms: Nombre de pièces — visible tous
(@StepDetails, 'rooms', N'Nombre de pièces', 'number', N'0',
 '', '', N'pièce(s)',
 N'Ne comptez que les pièces de séjour ou chambres, hors cuisine, salle d''eau, WC, couloirs, caves et dépendances.',
 1, 6, 1, NULL, '',
 N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- bedrooms: Nombre de chambres — visible tous
(@StepDetails, 'bedrooms', N'Nombre de chambres', 'select', N'Choisissez',
 N'[{"value":"0","label":"0 (Studio)"},{"value":"1","label":"1"},{"value":"2","label":"2"},{"value":"3","label":"3"},{"value":"4","label":"4"},{"value":"5","label":"5"},{"value":"6","label":"6"},{"value":"7","label":"7+"}]',
 '', '',
 N'Indiquez 0 pour un studio.',
 1, 7, 1, NULL, '',
 N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- bathrooms: Nombre de salles de bain — visible tous
(@StepDetails, 'bathrooms', N'Nombre de salles de bain', 'select', N'Choisissez',
 N'[{"value":"1","label":"1"},{"value":"2","label":"2"},{"value":"3","label":"3"},{"value":"4","label":"4+"},{"value":"autre","label":"Autre"}]',
 '', '', '', 0, 8, 1, NULL, '',
 N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- levels: Nombre de niveaux — maison uniquement
(@StepDetails, 'levels', N'Nombre de niveaux', 'number', N'Ex: 2',
 '', '', N'niveau(x)',
 N'Indiquez 1 pour une maison de plain-pied.',
 0, 9, 1, NULL, '',
 N'{"field":"propertyType","values":["maison"]}'),

-- floor: Étage — maison, appartement, autre
(@StepDetails, 'floor', N'Étage de votre bien', 'number', N'Ex: 3',
 '', '0', N'ème étage',
 N'Indiquez 0 pour un rez-de-chaussée.',
 0, 10, 1, NULL, '',
 N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- totalFloors: Nombre d'étages dans l'immeuble — appartement uniquement
(@StepDetails, 'totalFloors', N'Nombre d''étages dans l''immeuble', 'number', N'Ex: 5',
 '', '', N'étage(s)', '', 0, 11, 1, NULL, '',
 N'{"field":"propertyType","values":["appartement"]}'),

-- elevator: Ascenseur — appartement uniquement
(@StepDetails, 'elevator', N'Ascenseur', 'toggle', '', '', 'false', '', '', 0, 12, 1, NULL, '',
 N'{"field":"propertyType","values":["appartement"]}'),

-- exposure: Exposition — maison, appartement
(@StepDetails, 'exposure', N'Exposition', 'select', N'Choisissez',
 N'[{"value":"nord","label":"Nord"},{"value":"sud","label":"Sud"},{"value":"est","label":"Est"},{"value":"ouest","label":"Ouest"},{"value":"nord_est","label":"Nord-Est"},{"value":"nord_ouest","label":"Nord-Ouest"},{"value":"sud_est","label":"Sud-Est"},{"value":"sud_ouest","label":"Sud-Ouest"}]',
 '', '', '', 0, 13, 1, NULL, '',
 N'{"field":"propertyType","values":["maison","appartement"]}'),

-- propertyNature: Nature du bien — maison uniquement
(@StepDetails, 'propertyNature', N'Nature du bien', 'multiselect', '',
 N'[{"value":"villa","label":"Villa"},{"value":"individuelle","label":"Maison individuelle"},{"value":"ville","label":"Maison de ville"},{"value":"plain_pied","label":"Maison de plain-pied"},{"value":"ferme","label":"Ferme"},{"value":"mitoyenne","label":"Maison mitoyenne"},{"value":"autre","label":"Autre"}]',
 '', '', '', 0, 14, 1, NULL, '',
 N'{"field":"propertyType","values":["maison"]}'),

-- exterior: Extérieur — maison, appartement, autre
(@StepDetails, 'exterior', N'Extérieur', 'multiselect', '',
 N'[{"value":"balcon","label":"Balcon"},{"value":"terrasse","label":"Terrasse"},{"value":"jardin","label":"Jardin"},{"value":"piscine","label":"Piscine"},{"value":"autre","label":"Autre"}]',
 '', '', '', 0, 15, 1, NULL, '',
 N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- parking: Places de parking — maison, appartement, autre
(@StepDetails, 'parking', N'Places de parking', 'select', N'Choisissez',
 N'[{"value":"0","label":"0"},{"value":"1","label":"1"},{"value":"2","label":"2"},{"value":"3","label":"3"},{"value":"4","label":"4 et plus"}]',
 '', '', '', 0, 16, 1, NULL, '',
 N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- smokingPolicy: Statut fumeur — tous
(@StepDetails, 'smokingPolicy', N'Statut fumeur', 'select', N'Choisissez',
 N'[{"value":"autorise","label":"Autorisé"},{"value":"interdit","label":"Interdit"},{"value":"chambre","label":"Dans la chambre uniquement"}]',
 '', '', '', 0, 17, 1, NULL, '',
 N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- petsAllowed: Animaux acceptés (toggle) — tous
(@StepDetails, 'petsAllowed', N'Animaux acceptés', 'toggle', '', '', 'false', '', '', 0, 18, 1, NULL, '',
 N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- constructionYear: Année de construction — tous
(@StepDetails, 'constructionYear', N'Année de construction', 'year', N'AAAA',
 '', '', '', '', 0, 19, 1, 4, '',
 N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- availableFrom: Disponible à partir de — tous
(@StepDetails, 'availableFrom', N'Disponible à partir de', 'date_month', N'MM/AAAA',
 '', '', '', '', 0, 20, 1, 7, '',
 N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- heatingType: Type de chauffage (Individuel/Collectif) — appartement uniquement
(@StepDetails, 'heatingType', N'Type de chauffage', 'select', N'Choisissez',
 N'[{"value":"individuel","label":"Individuel"},{"value":"collectif","label":"Collectif"}]',
 '', '', '', 0, 21, 1, NULL, '',
 N'{"field":"propertyType","values":["appartement"]}'),

-- heatingMode: Mode de chauffage — maison, appartement
(@StepDetails, 'heatingMode', N'Mode de chauffage', 'multiselect', '',
 N'[{"value":"electricite","label":"Électricité"},{"value":"fioul","label":"Fioul"},{"value":"gaz","label":"Gaz"},{"value":"solaire","label":"Solaire"},{"value":"autre","label":"Autre"}]',
 '', '', '', 0, 22, 1, NULL, '',
 N'{"field":"propertyType","values":["maison","appartement"]}');

-- ============================================================
-- 6. Description step (AI enabled)
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
    Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
VALUES
(@StepDesc, 'description', N'Titre de l''annonce', 'text_counter',
 N'Ex: Chambre privee dans bel appartement lumineux Tunis',
 '', '', '', '', 1, 1, 1, 200, '', ''),
(@StepDesc, 'annonce_description', N'Description de l''annonce', 'textarea_counter',
 N'Decrivez votre colocation : ambiance, regles de vie, equipements, proximite des transports...',
 '', '', '', 'ai_enabled', 1, 2, 1, 4000, '', '');

-- ============================================================
-- 7. Price step — same as location (loyer + dépôt)
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
    Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
VALUES
(@StepPrice, 'monthlyRent', N'Loyer mensuel charges comprises *', 'number', N'Ex: 400',
 '', '', N'TND', '', 1, 1, 1, NULL, '', ''),
(@StepPrice, 'deposit', N'Dépôt de garantie', 'number', N'Ex: 400',
 '', '', N'TND',
 N'Ce montant est limité à 1 mois de loyer hors charges pour un logement non-meublé, et à 2 mois de loyer hors charges pour un logement meublé.',
 0, 2, 1, NULL, '', '');

-- ============================================================
-- 8. Contact step
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
    Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
VALUES
(@StepContact, 'email',     N'Email',            'email',  '', '', '', '', '', 1, 1, 1, NULL, '', ''),
(@StepContact, 'phone',     N'Téléphone',         'phone',  '', '', '', '', '', 1, 2, 1, NULL, '', ''),
(@StepContact, 'hidePhone', N'Masquer le numéro', 'toggle', '', '', 'false', '', '', 0, 3, 1, NULL, '', '');

PRINT 'Script 034: Workflow "Colocation" created successfully.';
