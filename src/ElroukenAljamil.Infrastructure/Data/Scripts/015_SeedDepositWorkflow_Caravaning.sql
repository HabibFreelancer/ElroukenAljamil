-- ============================================
-- 015: Seed Deposit Workflow for "Véhicules > Caravaning"
-- Order: title -> photos -> details -> description -> price -> location -> contact
-- ============================================

DECLARE @CategoryId INT = 19;

IF NOT EXISTS (SELECT 1 FROM DepositWorkflows WHERE CategoryId = @CategoryId)
BEGIN
    INSERT INTO DepositWorkflows (CategoryId, Name, Description, IsActive, CreatedAt)
    VALUES (@CategoryId, N'Depot Vehicules Caravaning', N'Workflow pour deposer une annonce caravaning / camping-car', 1, GETUTCDATE());

    DECLARE @WorkflowId INT = SCOPE_IDENTITY();

    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired, IsActive) VALUES
    (@WorkflowId, 1, N'Commencons par l''essentiel !', N'* champs obligatoires', 'title', 1, 1),
    (@WorkflowId, 2, N'Ajoutez des photos', N'Faites glisser vos photos pour changer leur ordre', 'photos', 0, 1),
    (@WorkflowId, 3, N'Dites-nous en plus', N'Renseignez les caracteristiques de votre vehicule.', 'details', 1, 1),
    (@WorkflowId, 4, N'Decrivez votre bien !', N'Une bonne description augmente vos chances de vendre rapidement.', 'description', 1, 1),
    (@WorkflowId, 5, N'Quel est votre prix ?', N'Indiquez le prix de vente.', 'price', 1, 1),
    (@WorkflowId, 6, N'Ou se situe votre bien ?', N'Indiquez la localisation pour etre trouve facilement.', 'location', 1, 1),
    (@WorkflowId, 7, N'Vos coordonnees', N'Verifiez vos informations de contact avant publication.', 'contact', 1, 1);

    DECLARE @StepTitle INT, @StepDetails INT, @StepDesc INT, @StepPrice INT, @StepLocation INT, @StepContact INT;
    SELECT @StepTitle = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'title';
    SELECT @StepDetails = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'details';
    SELECT @StepDesc = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'description';
    SELECT @StepPrice = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'price';
    SELECT @StepLocation = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'location';
    SELECT @StepContact = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'contact';

    -- Step: Title
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepTitle, 'title', N'Titre de l''annonce', 'text', N'Ex: Camping-car Chausson 2020', '', '', '', '', 1, 1, 1, 200, '');

    -- Step: Details (6 fields)
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepDetails, 'vehicleType', N'Type', 'select', N'Choisissez',
     N'[{"value":"alcove","label":"Camping car a alcove"},{"value":"semi_integre","label":"Camping car semi integre"},{"value":"integre","label":"Camping car integre"},{"value":"fourgonnette","label":"Fourgonnette"},{"value":"caravane","label":"Caravane"},{"value":"autre","label":"Autre caravaning"}]',
     '', '', '', 1, 1, 1, NULL, ''),
    (@StepDetails, 'model', N'Modele', 'dependent_select', N'Choisissez', '', '', '', '', 1, 2, 1, NULL, ''),
    (@StepDetails, 'year', N'Annee modele', 'select', N'Choisissez',
     N'[{"value":"2024","label":"2024"},{"value":"2023","label":"2023"},{"value":"2022","label":"2022"},{"value":"2021","label":"2021"},{"value":"2020","label":"2020"},{"value":"2019","label":"2019"},{"value":"2018","label":"2018"},{"value":"2017","label":"2017"},{"value":"2016","label":"2016"},{"value":"2015","label":"2015"},{"value":"avant_2015","label":"Avant 2015"}]',
     '', '', '', 1, 3, 1, NULL, ''),
    (@StepDetails, 'firstCirculation', N'Date de premiere mise en circulation', 'date_month', N'MM/AAAA', '', '', '',
     N'Mention obligatoire dans le cadre de la vente de vehicule d''occasion.', 0, 4, 1, 7, ''),
    (@StepDetails, 'technicalControl', N'Date de fin de validite du controle technique', 'date_month', N'MM/AAAA', '', '', '', '', 0, 5, 1, 7, ''),
    (@StepDetails, 'mileage', N'Kilometrage', 'number', N'0', '', '', 'km', '', 1, 6, 1, NULL, '');

    -- Step: Description
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepDesc, 'description', N'Titre de l''annonce', 'text_counter', N'Ex: Camping-car Chausson 2020 profil', '', '', '', '', 1, 1, 1, 200, ''),
    (@StepDesc, 'annonce_description', N'Description de l''annonce', 'textarea_counter', N'Decrivez votre vehicule : equipements, couchages, kilometrage...', '', '', '', '', 1, 2, 1, 4000, '');

    -- Step: Price
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepPrice, 'price', N'Votre prix de vente', 'price_gauge', N'Ex: 35000', '', '', 'TND', '', 1, 1, 1, NULL, '');

    -- Step: Location
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepLocation, 'address', N'Adresse', 'address', N'Tapez votre adresse...', '', '', '', N'Completez votre adresse pour etre trouve facilement.', 1, 1, 1, NULL, '');

    -- Step: Contact
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepContact, 'email', N'Email', 'email', N'', '', '', '', '', 1, 1, 1, NULL, ''),
    (@StepContact, 'phone', N'Telephone', 'phone', N'', '', '', '', '', 1, 2, 1, NULL, ''),
    (@StepContact, 'hidePhone', N'Masquer le numero', 'toggle', '', '', 'false', '', '', 0, 3, 1, NULL, '');

    PRINT 'Workflow "Caravaning" created with 7 steps and 13 fields.';
END
ELSE
    PRINT 'Workflow already exists.';
