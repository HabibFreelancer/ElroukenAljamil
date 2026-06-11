-- ============================================
-- 013: Seed Deposit Workflow for "Véhicules > Motos"
-- Order: title -> photos -> details -> description -> price -> location -> contact
-- ============================================

DECLARE @CategoryId INT;
SELECT @CategoryId = Id FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Name LIKE N'%hicule%') AND Name = N'Motos';

IF @CategoryId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM DepositWorkflows WHERE CategoryId = @CategoryId)
BEGIN
    INSERT INTO DepositWorkflows (CategoryId, Name, Description, IsActive, CreatedAt)
    VALUES (@CategoryId, N'Depot Vehicules Motos', N'Workflow complet pour deposer une annonce moto', 1, GETUTCDATE());

    DECLARE @WorkflowId INT = SCOPE_IDENTITY();

    -- Insert 7 Steps
    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired, IsActive) VALUES
    (@WorkflowId, 1, N'Commencons par l''essentiel !', N'* champs obligatoires', 'title', 1, 1),
    (@WorkflowId, 2, N'Ajoutez des photos', N'Faites glisser vos photos pour changer leur ordre', 'photos', 0, 1),
    (@WorkflowId, 3, N'Dites-nous en plus', N'Renseignez les caracteristiques de votre moto.', 'details', 1, 1),
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
    VALUES (@StepTitle, 'title', N'Titre de l''annonce', 'text', N'Ex: Yamaha MT-07 2022 ABS', '', '', '', '', 1, 1, 1, 200, '');

    -- Step: Details (11 fields)
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepDetails, 'brand', N'Marque', 'select', N'Choisissez',
     N'[{"value":"yamaha","label":"Yamaha"},{"value":"honda","label":"Honda"},{"value":"bmw","label":"BMW"},{"value":"kawasaki","label":"Kawasaki"},{"value":"suzuki","label":"Suzuki"},{"value":"triumph","label":"Triumph"},{"value":"ducati","label":"Ducati"},{"value":"harley","label":"Harley-Davidson"},{"value":"ktm","label":"KTM"},{"value":"aprilia","label":"Aprilia"},{"value":"mv_agusta","label":"MV Agusta"},{"value":"benelli","label":"Benelli"},{"value":"royal_enfield","label":"Royal Enfield"},{"value":"husqvarna","label":"Husqvarna"},{"value":"indian","label":"Indian"},{"value":"piaggio","label":"Piaggio"},{"value":"vespa","label":"Vespa"},{"value":"sym","label":"SYM"},{"value":"kymco","label":"Kymco"},{"value":"mbk","label":"MBK"},{"value":"peugeot","label":"Peugeot"},{"value":"autre","label":"Autre"}]',
     '', '', '', 1, 1, 1, NULL, ''),
    (@StepDetails, 'model', N'Modele', 'text', N'Ex: MT-07, CB650R, R1250GS...', '', '', '', '', 1, 2, 1, 100, ''),
    (@StepDetails, 'year', N'Annee modele', 'select', N'Choisissez',
     N'[{"value":"2024","label":"2024"},{"value":"2023","label":"2023"},{"value":"2022","label":"2022"},{"value":"2021","label":"2021"},{"value":"2020","label":"2020"},{"value":"2019","label":"2019"},{"value":"2018","label":"2018"},{"value":"2017","label":"2017"},{"value":"2016","label":"2016"},{"value":"2015","label":"2015"},{"value":"avant_2015","label":"Avant 2015"}]',
     '', '', '', 1, 3, 1, NULL, ''),
    (@StepDetails, 'vehicleType', N'Type de vehicule', 'select', N'Choisissez',
     N'[{"value":"moto","label":"Moto"},{"value":"scooter","label":"Scooter"},{"value":"quad","label":"Quad"},{"value":"sidecar","label":"Side-car"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 4, 1, NULL, ''),
    (@StepDetails, 'motoType', N'Type de moto', 'select', N'Choisissez',
     N'[{"value":"sportive","label":"Sportive"},{"value":"roadster","label":"Roadster"},{"value":"trail","label":"Trail / Enduro"},{"value":"custom","label":"Custom / Cruiser"},{"value":"touring","label":"Touring / GT"},{"value":"cafe_racer","label":"Cafe Racer"},{"value":"cross","label":"Cross / Supermotard"},{"value":"classique","label":"Classique / Vintage"},{"value":"utilitaire","label":"Utilitaire"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 5, 1, NULL, ''),
    (@StepDetails, 'cylindree', N'Cylindree', 'select', N'Choisissez',
     N'[{"value":"moins_50","label":"Moins de 50 cm3"},{"value":"50_125","label":"50 - 125 cm3"},{"value":"125_600","label":"125 - 600 cm3"},{"value":"600_900","label":"600 - 900 cm3"},{"value":"900_1200","label":"900 - 1200 cm3"},{"value":"plus_1200","label":"Plus de 1200 cm3"}]',
     '', '', '', 0, 6, 1, NULL, ''),
    (@StepDetails, 'mileage', N'Kilometrage', 'number', N'0', '', '', 'km', '', 1, 7, 1, NULL, ''),
    (@StepDetails, 'license', N'Type de permis', 'radio', N'',
     N'[{"value":"a","label":"Permis A"},{"value":"a2","label":"Permis A2"},{"value":"a1","label":"Permis A1 (125 cm3)"},{"value":"sans","label":"Sans permis"}]',
     '', '', '', 0, 8, 1, NULL, ''),
    (@StepDetails, 'color', N'Couleur', 'select', N'Choisissez',
     N'[{"value":"noir","label":"Noir"},{"value":"blanc","label":"Blanc"},{"value":"gris","label":"Gris"},{"value":"rouge","label":"Rouge"},{"value":"bleu","label":"Bleu"},{"value":"vert","label":"Vert"},{"value":"orange","label":"Orange"},{"value":"jaune","label":"Jaune"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 9, 1, NULL, ''),
    (@StepDetails, 'vehicleState', N'Etat du vehicule', 'select', N'Choisissez',
     N'[{"value":"neuf","label":"Neuf"},{"value":"tres_bon","label":"Tres bon etat"},{"value":"bon","label":"Bon etat"},{"value":"usure","label":"Traces d''usure normales"},{"value":"accident","label":"Accidente / Pour pieces"}]',
     '', '', '', 0, 10, 1, NULL, ''),
    (@StepDetails, 'equipment', N'Equipements', 'multiselect', N'Selectionnez',
     N'[{"value":"abs","label":"ABS"},{"value":"antidemarrage","label":"Antidemarrage"},{"value":"bequille_centrale","label":"Bequille centrale"},{"value":"demarreur_elec","label":"Demarreur electrique"},{"value":"indicateur_rapport","label":"Indicateur de rapport engage"},{"value":"jauge_essence","label":"Jauge a essence"},{"value":"poignees_chauffantes","label":"Poignees chauffantes"},{"value":"topcase","label":"Top case / Valises"},{"value":"alarme","label":"Alarme"},{"value":"carnet_entretien","label":"Carnet d''entretien disponible"}]',
     '', '', '', 0, 11, 1, NULL, '');

    -- Step: Description
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepDesc, 'description', N'Titre de l''annonce', 'text_counter', N'Ex: Yamaha MT-07 2022 ABS 5000km', '', '', '', '', 1, 1, 1, 200, ''),
    (@StepDesc, 'annonce_description', N'Description de l''annonce', 'textarea_counter', N'Decrivez votre moto : equipements, entretien, historique...', '', '', '', '', 1, 2, 1, 4000, '');

    -- Step: Price
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepPrice, 'price', N'Votre prix de vente', 'price_gauge', N'Ex: 8000', '', '', 'TND', '', 1, 1, 1, NULL, '');

    -- Step: Location
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepLocation, 'address', N'Adresse', 'address', N'Tapez votre adresse...', '', '', '', N'Completez votre adresse pour etre trouve facilement.', 1, 1, 1, NULL, '');

    -- Step: Contact
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepContact, 'email', N'Email', 'email', N'', '', '', '', '', 1, 1, 1, NULL, ''),
    (@StepContact, 'phone', N'Telephone', 'phone', N'', '', '', '', '', 1, 2, 1, NULL, ''),
    (@StepContact, 'hidePhone', N'Masquer le numero', 'toggle', '', '', 'false', '', '', 0, 3, 1, NULL, '');

    PRINT 'Workflow "Vehicules > Motos" created with 7 steps and 17 fields.';
END
ELSE
    PRINT 'Workflow already exists or category not found.';
