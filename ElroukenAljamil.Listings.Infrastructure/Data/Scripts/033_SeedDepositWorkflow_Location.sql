-- ============================================
-- 033: Seed Deposit Workflow for "Immobilier > Locations"
-- Based on the Ventes immobilières workflow (CategoryId=1)
-- Differences:
--   1. Details step: add "furnished" radio field (Meublé / Non meublé)
--      visible for maison + appartement
--   2. Price step: replace price_gauge with monthlyRent + deposit fields
-- ============================================

-- Resolve Locations parent category ID
DECLARE @LocationCategoryId INT = (
    SELECT Id FROM Categories
    WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'immobilier')
      AND Name = N'Locations'
      AND ParentCategoryId IS NULL
);

IF @LocationCategoryId IS NULL
BEGIN
    PRINT 'ERROR: Category "Locations" not found.';
    RETURN;
END

PRINT 'Locations CategoryId = ' + CAST(@LocationCategoryId AS VARCHAR);

-- Skip if workflow already exists
IF EXISTS (SELECT 1 FROM DepositWorkflows WHERE CategoryId = @LocationCategoryId)
BEGIN
    PRINT 'Workflow for Locations already exists. Skipping.';
    RETURN;
END

-- ============================================================
-- 1. Create the workflow
-- ============================================================
INSERT INTO DepositWorkflows (CategoryId, Name, Description, IsActive, CreatedAt)
VALUES (@LocationCategoryId, N'Depot Immobilier Location', N'Workflow pour deposer une annonce de location immobiliere', 1, GETUTCDATE());

DECLARE @WorkflowId INT = SCOPE_IDENTITY();

-- ============================================================
-- 2. Create 7 steps (same keys as vente, different titles where needed)
-- ============================================================
INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired, IsActive) VALUES
(@WorkflowId, 1, N'Commencons par l''essentiel !',      N'* champs obligatoires',                                                  'title',       1, 1),
(@WorkflowId, 2, N'Ou se situe votre bien ?',            N'Indiquez la localisation de votre bien.',                                'location',    1, 1),
(@WorkflowId, 3, N'Dites-nous en plus',                  N'Selectionnez le type de bien et renseignez les criteres.',              'details',     1, 1),
(@WorkflowId, 4, N'Ajoutez des photos',                  N'Les annonces avec photos recoivent plus de contacts.',                  'photos',      0, 1),
(@WorkflowId, 5, N'Decrivez votre bien !',               N'Une bonne description augmente vos chances de louer rapidement.',       'description', 1, 1),
(@WorkflowId, 6, N'Quel est votre loyer ?',              N'Indiquez le loyer et les conditions financieres.',                      'price',       1, 1),
(@WorkflowId, 7, N'Vos coordonnees',                     N'Verifiez vos informations de contact avant publication.',               'contact',     1, 1);

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
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
VALUES (@StepTitle, 'title', N'Titre de l''annonce', 'text', N'Ex: Appartement 3 pieces 75m2 Tunis', '', '', '', '', 1, 1, 1, 200, '', '');

-- ============================================================
-- 4. Location step (address + map)
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
VALUES (@StepLocation, 'address', N'Adresse', 'address', N'Tapez votre adresse...', '', '', '',
    N'Completez votre adresse et les personnes utilisant la recherche autour de soi trouveront plus facilement votre annonce.',
    1, 1, 1, NULL, '', '');

-- ============================================================
-- 5. Details step — same fields as vente + furnished field
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition) VALUES

-- propertyType selector
(@StepDetails, 'propertyType',   N'Choisissez votre type de bien', 'pills', '',
 N'[{"value":"maison","label":"Maison"},{"value":"appartement","label":"Appartement"},{"value":"terrain","label":"Terrain"},{"value":"parking","label":"Parking"},{"value":"autre","label":"Autre"}]',
 '', '', '', 1, 1, 1, NULL, '', ''),

-- Critères indispensables
(@StepDetails, 'surface',        N'Surface habitable', 'number', N'0', '', '', N'm2',
 N'Comptez les surfaces interieures habitables.',
 1, 2, 1, NULL, '', N'{"field":"propertyType","values":["maison","appartement","terrain","autre"]}'),

(@StepDetails, 'rooms',          N'Nombre de pieces', 'number', N'0', '', '', N'piece(s)',
 N'Pieces de sejour ou chambres uniquement.',
 1, 3, 1, NULL, '', N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

(@StepDetails, 'bedrooms',       N'Nombre de chambres', 'select', N'Choisissez',
 N'[{"value":"0","label":"0 (Studio)"},{"value":"1","label":"1"},{"value":"2","label":"2"},{"value":"3","label":"3"},{"value":"4","label":"4"},{"value":"5","label":"5"},{"value":"6","label":"6"},{"value":"7","label":"7+"}]',
 '', '', N'Indiquez 0 pour un studio.',
 1, 4, 1, NULL, '', N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

(@StepDetails, 'bathrooms',      N'Nombre de salles de bain', 'select', N'Choisissez',
 N'[{"value":"1","label":"1"},{"value":"2","label":"2"},{"value":"3","label":"3"},{"value":"4","label":"4+"},{"value":"autre","label":"Autre"}]',
 '', '', '',
 0, 5, 1, NULL, '', N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

-- furnished: LOCATION-SPECIFIC — visible for maison + appartement
(@StepDetails, 'furnished',      N'Ce bien est :', 'radio', '',
 N'[{"value":"non_meuble","label":"Non meublé"},{"value":"meuble","label":"Meublé"}]',
 'non_meuble', '', '',
 1, 6, 1, NULL, '', N'{"field":"propertyType","values":["maison","appartement"]}'),

(@StepDetails, 'cuisine',        N'Cuisine', 'multiselect', '',
 N'[{"value":"equipee","label":"Équipée"},{"value":"ouverte","label":"Ouverte"},{"value":"separee","label":"Séparée"}]',
 '', '', '',
 0, 7, 1, NULL, '', N'{"field":"propertyType","values":["maison","appartement"]}'),

(@StepDetails, 'levels',         N'Nombre de niveaux', 'number', N'Ex: 2', '', '', N'niveau(x)',
 N'Indiquez 1 pour une maison de plain-pied.',
 0, 8, 1, NULL, '', N'{"field":"propertyType","values":["maison"]}'),

(@StepDetails, 'condition',      N'État du bien', 'select', N'Choisissez',
 N'[{"value":"tres_bon","label":"Très bon état"},{"value":"bon","label":"Bon état"},{"value":"renove","label":"Rénové"},{"value":"rafraichir","label":"À rafraîchir"},{"value":"travaux","label":"Travaux à prévoir"}]',
 '', '', '',
 0, 9, 1, NULL, '', N'{"field":"propertyType","values":["maison","appartement","parking","autre"]}'),

(@StepDetails, 'floor',          N'Étage de votre bien', 'number', N'Ex: 3', '', '0', '', N'Indiquez 0 pour un rez-de-chaussée.',
 0, 10, 1, NULL, '', N'{"field":"propertyType","values":["appartement"]}'),

(@StepDetails, 'totalFloors',    N'Nombre d''étages dans l''immeuble', 'number', N'Ex: 5', '', '', N'étage(s)', '',
 0, 11, 1, NULL, '', N'{"field":"propertyType","values":["appartement"]}'),

(@StepDetails, 'elevator',       N'Ascenseur', 'toggle', '', '', 'false', '', '',
 0, 12, 1, NULL, '', N'{"field":"propertyType","values":["appartement"]}'),

(@StepDetails, 'constructionYear', N'Année de construction', 'year', N'AAAA', '', '', '', '',
 0, 13, 1, 4, '', N'{"field":"propertyType","values":["maison","appartement","parking","autre"]}'),

-- section divider
(@StepDetails, 'section_atouts', N'Les atouts qui font la différence', 'section_title', '', '', '', '', '',
 0, 14, 1, NULL, '', N'{"field":"propertyType","values":["maison","appartement"]}'),

(@StepDetails, 'propertyNature', N'Nature du bien', 'multiselect', '',
 N'[{"value":"villa","label":"Villa"},{"value":"individuelle","label":"Maison individuelle"},{"value":"ville","label":"Maison de ville"},{"value":"plain_pied","label":"Maison de plain-pied"},{"value":"ferme","label":"Ferme"},{"value":"mitoyenne","label":"Maison mitoyenne"},{"value":"autre","label":"Autre"}]',
 '', '', '',
 0, 15, 1, NULL, '', N'{"field":"propertyType","values":["maison"]}'),

(@StepDetails, 'terrainNature',  N'Nature du terrain', 'select', N'Choisissez',
 N'[{"value":"jardin","label":"Jardin"},{"value":"constructible","label":"Terrain constructible"},{"value":"agricole","label":"Terrain agricole"},{"value":"autre","label":"Autre"}]',
 '', '', '',
 0, 15, 1, NULL, '', N'{"field":"propertyType","values":["terrain"]}'),

(@StepDetails, 'parkingNature',  N'Nature du bien', 'select', N'Choisissez',
 N'[{"value":"exterieur","label":"Stationnement extérieur"},{"value":"couvert","label":"Stationnement couvert"},{"value":"box","label":"Box ou garage fermé"},{"value":"autre","label":"Autre"}]',
 '', '', '',
 0, 15, 1, NULL, '', N'{"field":"propertyType","values":["parking"]}'),

(@StepDetails, 'features',       N'Caractéristiques', 'multiselect', '',
 N'[{"value":"acces_pmr","label":"Accès PMR"},{"value":"chauffage_sol","label":"Chauffage au sol"},{"value":"baignoire","label":"Baignoire"},{"value":"toilettes","label":"Plusieurs toilettes"},{"value":"autre","label":"Autre"}]',
 '', '', '',
 0, 16, 1, NULL, '', N'{"field":"propertyType","values":["maison","appartement","parking","autre"]}'),

(@StepDetails, 'landSurface',    N'Surface totale du terrain', 'number', N'0', '', '', N'm2',
 N'Incluez la surface au sol de votre maison.',
 0, 17, 1, NULL, '', N'{"field":"propertyType","values":["maison","terrain","autre"]}'),

(@StepDetails, 'parking',        N'Places de parking', 'select', N'Choisissez',
 N'[{"value":"0","label":"0"},{"value":"1","label":"1"},{"value":"2","label":"2"},{"value":"3","label":"3"},{"value":"4","label":"4"},{"value":"5","label":"5"},{"value":"6","label":"6 et plus"},{"value":"autre","label":"Autre"}]',
 '', '', '',
 0, 18, 1, NULL, '', N'{"field":"propertyType","values":["maison","appartement","parking","autre"]}'),

(@StepDetails, 'heatingMode',    N'Mode de chauffage', 'multiselect', '',
 N'[{"value":"electricite","label":"Électricité"},{"value":"fioul","label":"Fioul"},{"value":"gaz","label":"Gaz"},{"value":"solaire","label":"Solaire"},{"value":"autre","label":"Autre"}]',
 '', '', '',
 0, 19, 1, NULL, '', N'{"field":"propertyType","values":["maison","appartement","autre"]}'),

(@StepDetails, 'exterior',       N'Extérieur', 'multiselect', '',
 N'[{"value":"balcon","label":"Balcon"},{"value":"terrasse","label":"Terrasse"},{"value":"jardin","label":"Jardin"},{"value":"piscine","label":"Piscine"},{"value":"autre","label":"Autre"}]',
 '', '', '',
 0, 20, 1, NULL, '', N'{"field":"propertyType","values":["appartement","autre"]}'),

(@StepDetails, 'exposure',       N'Exposition', 'select', N'Choisissez',
 N'[{"value":"nord","label":"Nord"},{"value":"sud","label":"Sud"},{"value":"est","label":"Est"},{"value":"ouest","label":"Ouest"},{"value":"nord_est","label":"Nord-Est"},{"value":"nord_ouest","label":"Nord-Ouest"},{"value":"sud_est","label":"Sud-Est"},{"value":"sud_ouest","label":"Sud-Ouest"}]',
 '', '', '',
 0, 21, 1, NULL, '', N'{"field":"propertyType","values":["maison","appartement"]}'),

(@StepDetails, 'availableFrom',  N'Disponible à partir de', 'date_month', N'MM/AAAA', '', '', '', '',
 0, 22, 1, 7, '', '');

-- ============================================================
-- 6. Description step (AI enabled)
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition) VALUES
(@StepDesc, 'description',       N'Titre de l''annonce', 'text_counter',
 N'Ex: Appartement 3 pieces lumineux centre-ville', '', '', '', '',
 1, 1, 1, 200, '', ''),
(@StepDesc, 'annonce_description', N'Description de l''annonce', 'textarea_counter',
 N'Decrivez votre bien : superficie, pieces, equipements, proximite des transports...', '', '', '', 'ai_enabled',
 1, 2, 1, 4000, '', '');

-- ============================================================
-- 7. Price step — LOCATION-SPECIFIC
--    monthlyRent + deposit + legal indication
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition) VALUES

(@StepPrice, 'monthlyRent',   N'Loyer mensuel charges comprises *', 'number', N'Ex: 800', '', '', N'TND',
 '', 1, 1, 1, NULL, '', ''),

(@StepPrice, 'deposit',       N'Dépôt de garantie', 'number', N'Ex: 800', '', '', N'TND',
 N'Ce montant est limité à 1 mois de loyer hors charges pour un logement non-meublé, et à 2 mois de loyer hors charges pour un logement meublé.',
 0, 2, 1, NULL, '', '');

-- ============================================================
-- 8. Contact step
-- ============================================================
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition) VALUES
(@StepContact, 'email',     N'Email',           'email',  '', '', '', '', '', 1, 1, 1, NULL, '', ''),
(@StepContact, 'phone',     N'Téléphone',        'phone',  '', '', '', '', '', 1, 2, 1, NULL, '', ''),
(@StepContact, 'hidePhone', N'Masquer le numéro','toggle', '', '', 'false', '', '', 0, 3, 1, NULL, '', '');

PRINT 'Script 033: Workflow "Immobilier Location" created successfully.';
