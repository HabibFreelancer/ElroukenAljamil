-- ============================================
-- 019: Seed Deposit Workflow for "Véhicules > Équipement moto"
-- Same structure but details step only has multiselect type field
-- ============================================

DECLARE @CategoryId INT = 25;

IF NOT EXISTS (SELECT 1 FROM DepositWorkflows WHERE CategoryId = @CategoryId)
BEGIN
    INSERT INTO DepositWorkflows (CategoryId, Name, Description, IsActive, CreatedAt)
    VALUES (@CategoryId, N'Depot Equipement Moto', N'Workflow pour deposer une annonce equipement moto', 1, GETUTCDATE());

    DECLARE @WorkflowId INT = SCOPE_IDENTITY();

    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired, IsActive) VALUES
    (@WorkflowId, 1, N'Commencons par l''essentiel !', N'* champs obligatoires', 'title', 1, 1),
    (@WorkflowId, 2, N'Ajoutez des photos', N'Faites glisser vos photos pour changer leur ordre', 'photos', 0, 1),
    (@WorkflowId, 3, N'Dites-nous en plus', N'Selectionnez le type d''equipement.', 'details', 1, 1),
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
    VALUES (@StepTitle, 'title', N'Titre de l''annonce', 'text', N'Ex: Casque Shoei GT-Air taille M', '', '', '', '', 1, 1, 1, 200, '');

    -- Step: Details (1 field - multiselect)
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepDetails, 'equipmentType', N'Type d''equipement', 'multiselect', N'Selectionnez',
     N'[{"value":"remplacement_reparation","label":"Remplacement et reparation de vehicules"},{"value":"pneus_jantes","label":"Pneus & jantes"},{"value":"vetements_moto","label":"Vetements moto"},{"value":"autre","label":"Autre"}]',
     '', '', '', 1, 1, 1, NULL, '');

    -- Step: Description
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepDesc, 'description', N'Titre de l''annonce', 'text_counter', N'Ex: Casque Shoei GT-Air noir mat taille M', '', '', '', '', 1, 1, 1, 200, ''),
    (@StepDesc, 'annonce_description', N'Description de l''annonce', 'textarea_counter', N'Decrivez votre equipement : marque, taille, etat, compatibilite...', '', '', '', '', 1, 2, 1, 4000, '');

    -- Step: Price
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepPrice, 'price', N'Votre prix de vente', 'price_gauge', N'Ex: 150', '', '', 'TND', '', 1, 1, 1, NULL, '');

    -- Step: Location
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepLocation, 'address', N'Adresse', 'address', N'Tapez votre adresse...', '', '', '', N'Completez votre adresse pour etre trouve facilement.', 1, 1, 1, NULL, '');

    -- Step: Contact
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepContact, 'email', N'Email', 'email', N'', '', '', '', '', 1, 1, 1, NULL, ''),
    (@StepContact, 'phone', N'Telephone', 'phone', N'', '', '', '', '', 1, 2, 1, NULL, ''),
    (@StepContact, 'hidePhone', N'Masquer le numero', 'toggle', '', '', 'false', '', '', 0, 3, 1, NULL, '');

    PRINT 'Workflow "Equipement Moto" created with 7 steps and 8 fields.';
END
ELSE
    PRINT 'Workflow already exists.';
