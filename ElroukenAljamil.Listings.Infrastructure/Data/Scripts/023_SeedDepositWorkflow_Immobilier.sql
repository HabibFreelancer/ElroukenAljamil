-- ============================================
-- 023: Seed Deposit Workflow for "Immobilier > Ventes immobilieres"
-- Order: title -> location -> details -> photos -> description -> price -> contact
-- ============================================

DECLARE @CategoryId INT = 1;

IF NOT EXISTS (SELECT 1 FROM DepositWorkflows WHERE CategoryId = @CategoryId)
BEGIN
    INSERT INTO DepositWorkflows (CategoryId, Name, Description, IsActive, CreatedAt)
    VALUES (@CategoryId, N'Depot Immobilier Vente', N'Workflow pour deposer une annonce immobiliere', 1, GETUTCDATE());

    DECLARE @WorkflowId INT = SCOPE_IDENTITY();

    -- 7 Steps in specific order: title -> location -> details -> photos -> description -> price -> contact
    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired, IsActive) VALUES
    (@WorkflowId, 1, N'Commencons par l''essentiel !', N'* champs obligatoires', 'title', 1, 1),
    (@WorkflowId, 2, N'Ou se situe votre bien ?', N'Indiquez la localisation de votre bien.', 'location', 1, 1),
    (@WorkflowId, 3, N'Dites-nous en plus', N'Selectionnez le type de bien et renseignez les criteres.', 'details', 1, 1),
    (@WorkflowId, 4, N'Ajoutez des photos', N'Les annonces avec photos recoivent plus de contacts.', 'photos', 0, 1),
    (@WorkflowId, 5, N'Decrivez votre bien !', N'Une bonne description augmente vos chances de vendre rapidement.', 'description', 1, 1),
    (@WorkflowId, 6, N'Quel est votre prix ?', N'Indiquez le prix de vente.', 'price', 1, 1),
    (@WorkflowId, 7, N'Vos coordonnees', N'Verifiez vos informations de contact avant publication.', 'contact', 1, 1);

    DECLARE @StepTitle INT, @StepLocation INT, @StepDetails INT, @StepPhotos INT, @StepDesc INT, @StepPrice INT, @StepContact INT;
    SELECT @StepTitle = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'title';
    SELECT @StepLocation = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'location';
    SELECT @StepDetails = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'details';
    SELECT @StepPhotos = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'photos';
    SELECT @StepDesc = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'description';
    SELECT @StepPrice = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'price';
    SELECT @StepContact = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'contact';

    -- Step: Title
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepTitle, 'title', N'Titre de l''annonce', 'text', N'Ex: Appartement 3 pieces 75m2 Tunis', '', '', '', '', 1, 1, 1, 200, '');

    -- Step: Location (address field - the helper text and street view toggle will be handled in frontend)
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepLocation, 'address', N'Adresse', 'address', N'Tapez votre adresse...', '', '', '',
     N'Completez votre adresse et les personnes utilisant la recherche autour de soi trouveront plus facilement votre annonce. Si vous ne souhaitez pas renseigner votre adresse exacte, indiquez votre rue sans donner le numero. Cette information ne sera conservee que le temps de la publication de votre annonce.', 1, 1, 1, NULL, '');

    -- Step: Details (type de bien + disponibilite)
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepDetails, 'propertyType', N'Choisissez votre type de bien', 'pills', N'',
     N'[{"value":"maison","label":"Maison"},{"value":"appartement","label":"Appartement"},{"value":"terrain","label":"Terrain"},{"value":"parking","label":"Parking"},{"value":"autre","label":"Autre"}]',
     '', '', '', 1, 1, 1, NULL, ''),
    (@StepDetails, 'surface', N'Surface habitable', 'number', N'0', '', '', 'm2', N'Comptez les surfaces interieures habitables d''une hauteur sous plafond de plus de 1,80m.', 1, 2, 1, NULL, ''),
    (@StepDetails, 'rooms', N'Nombre de pieces', 'number', N'0', '', '', 'piece(s)', N'Ne comptez que les pieces de sejour ou chambres, hors cuisine, salle d''eau, WC, couloirs, caves et dependances.', 1, 3, 1, NULL, ''),
    (@StepDetails, 'bedrooms', N'Nombre de chambres', 'select', N'Choisissez',
     N'[{"value":"0","label":"0 (Studio)"},{"value":"1","label":"1"},{"value":"2","label":"2"},{"value":"3","label":"3"},{"value":"4","label":"4"},{"value":"5","label":"5"},{"value":"6","label":"6"},{"value":"7","label":"7+"}]',
     '', '', N'Indiquez 0 pour un studio.', 1, 4, 1, NULL, ''),
    (@StepDetails, 'bathrooms', N'Nombre de salles de bain', 'select', N'Choisissez',
     N'[{"value":"1","label":"1"},{"value":"2","label":"2"},{"value":"3","label":"3"},{"value":"4","label":"4+"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 5, 1, NULL, ''),
    (@StepDetails, 'levels', N'Nombre de niveaux', 'number', N'1', '', '', '', N'Indiquez 1 pour une maison de plain-pied.', 0, 6, 1, NULL, ''),
    (@StepDetails, 'constructionYear', N'Annee de construction', 'number', N'', '', '', '', '', 0, 7, 1, NULL, ''),
    (@StepDetails, 'propertyNature', N'Nature du bien', 'select', N'Choisissez',
     N'[{"value":"villa","label":"Villa"},{"value":"individuelle","label":"Maison individuelle"},{"value":"ville","label":"Maison de ville"},{"value":"collective","label":"Residence collective"},{"value":"plain_pied","label":"Maison de plain-pied"},{"value":"ferme","label":"Ferme"},{"value":"mitoyenne","label":"Maison mitoyenne"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 8, 1, NULL, ''),
    (@StepDetails, 'condition', N'Etat du bien', 'select', N'Choisissez',
     N'[{"value":"tres_bon","label":"Tres bon etat"},{"value":"bon","label":"Bon etat"},{"value":"renove","label":"Renove"},{"value":"rafraichir","label":"A rafraichir"},{"value":"travaux","label":"Travaux a prevoir"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 9, 1, NULL, ''),
    (@StepDetails, 'features', N'Caracteristiques', 'multiselect', N'Selectionnez',
     N'[{"value":"acces_pmr","label":"Acces PMR"},{"value":"chauffage_sol","label":"Chauffage au sol"},{"value":"ancien","label":"Construction ancienne"},{"value":"baignoire","label":"Baignoire"},{"value":"recent","label":"Construction recente"},{"value":"toilettes","label":"Plusieurs toilettes"},{"value":"vendu_loue","label":"Vendu loue"},{"value":"batiment_classe","label":"Batiment classe"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 10, 1, NULL, ''),
    (@StepDetails, 'landSurface', N'Surface totale du terrain', 'number', N'0', '', '', 'm2', N'Incluez la surface au sol de votre maison.', 0, 11, 1, NULL, ''),
    (@StepDetails, 'parking', N'Places de parking', 'select', N'Choisissez',
     N'[{"value":"0","label":"0"},{"value":"1","label":"1"},{"value":"2","label":"2"},{"value":"3","label":"3"},{"value":"4","label":"4"},{"value":"5","label":"5"},{"value":"6","label":"6 et plus"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 12, 1, NULL, ''),
    (@StepDetails, 'heatingMode', N'Mode de chauffage', 'multiselect', N'Selectionnez',
     N'[{"value":"electricite","label":"Electricite"},{"value":"fioul","label":"Fioul"},{"value":"gaz","label":"Gaz"},{"value":"solaire","label":"Solaire"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 13, 1, NULL, ''),
    (@StepDetails, 'exterior', N'Exterieur', 'multiselect', N'Selectionnez',
     N'[{"value":"balcon","label":"Balcon"},{"value":"terrasse","label":"Terrasse"},{"value":"jardin","label":"Jardin"},{"value":"piscine","label":"Piscine"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 14, 1, NULL, ''),
    (@StepDetails, 'exposure', N'Exposition', 'select', N'Choisissez',
     N'[{"value":"nord","label":"Nord"},{"value":"sud","label":"Sud"},{"value":"est","label":"Est"},{"value":"ouest","label":"Ouest"},{"value":"nord_est","label":"Nord-Est"},{"value":"nord_ouest","label":"Nord-Ouest"},{"value":"sud_est","label":"Sud-Est"},{"value":"sud_ouest","label":"Sud-Ouest"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 15, 1, NULL, ''),
    (@StepDetails, 'availableFrom', N'Disponible a partir de', 'date_month', N'MM/AAAA', '', '', '', '', 0, 16, 1, 7, '');

    -- Step: Description
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepDesc, 'description', N'Titre de l''annonce', 'text_counter', N'Ex: Appartement 3 pieces lumineux centre-ville', '', '', '', '', 1, 1, 1, 200, ''),
    (@StepDesc, 'annonce_description', N'Description de l''annonce', 'textarea_counter', N'Decrivez votre bien : superficie, pieces, environnement, proximite...', '', '', '', '', 1, 2, 1, 4000, '');

    -- Step: Price
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepPrice, 'price', N'Votre prix de vente', 'price_gauge', N'Ex: 250000', '', '', 'TND', '', 1, 1, 1, NULL, '');

    -- Step: Contact
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepContact, 'email', N'Email', 'email', N'', '', '', '', '', 1, 1, 1, NULL, ''),
    (@StepContact, 'phone', N'Telephone', 'phone', N'', '', '', '', '', 1, 2, 1, NULL, ''),
    (@StepContact, 'hidePhone', N'Masquer le numero', 'toggle', '', '', 'false', '', '', 0, 3, 1, NULL, '');

    PRINT 'Workflow "Immobilier Vente" created with 7 steps and 22 fields.';
END
ELSE
    PRINT 'Workflow already exists.';
