-- ============================================
-- 016: Seed Deposit Workflow for "Véhicules > Utilitaires"
-- Same as Voitures + Carrosserie, Transmission, Finition, Version, Volume, PTAC, TVA, Classe emission
-- ============================================

DECLARE @CategoryId INT = 20;

IF NOT EXISTS (SELECT 1 FROM DepositWorkflows WHERE CategoryId = @CategoryId)
BEGIN
    INSERT INTO DepositWorkflows (CategoryId, Name, Description, IsActive, CreatedAt)
    VALUES (@CategoryId, N'Depot Vehicules Utilitaires', N'Workflow pour deposer une annonce utilitaire', 1, GETUTCDATE());

    DECLARE @WorkflowId INT = SCOPE_IDENTITY();

    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired, IsActive) VALUES
    (@WorkflowId, 1, N'Commencons par l''essentiel !', N'* champs obligatoires', 'title', 1, 1),
    (@WorkflowId, 2, N'Ajoutez des photos', N'Faites glisser vos photos pour changer leur ordre', 'photos', 0, 1),
    (@WorkflowId, 3, N'Dites-nous en plus', N'L''immatriculation de votre vehicule est obligatoire pour continuer.', 'details', 1, 1),
    (@WorkflowId, 4, N'Decrivez votre bien !', N'Une bonne description augmente vos chances de vendre rapidement.', 'description', 1, 1),
    (@WorkflowId, 5, N'Quel est votre prix ?', N'Indiquez le prix de vente de votre vehicule.', 'price', 1, 1),
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
    VALUES (@StepTitle, 'title', N'Titre de l''annonce', 'text', N'Ex: Renault Master 2020 Fourgon', '', '', '', '', 1, 1, 1, 200, '');

    -- Step: Details (27 fields - voiture base + utilitaire specifics)
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepDetails, 'immatriculation', N'Numero d''immatriculation', 'immatriculation', N'Ex: 123 TU 4567', '', '', '', N'Il ne sera pas visible sur l''annonce.', 1, 1, 1, 20, ''),
    (@StepDetails, 'brand', N'Marque', 'select', N'Choisissez',
     N'[{"value":"renault","label":"Renault"},{"value":"peugeot","label":"Peugeot"},{"value":"citroen","label":"Citroen"},{"value":"fiat","label":"Fiat"},{"value":"ford","label":"Ford"},{"value":"mercedes","label":"Mercedes"},{"value":"volkswagen","label":"Volkswagen"},{"value":"iveco","label":"Iveco"},{"value":"opel","label":"Opel"},{"value":"nissan","label":"Nissan"},{"value":"toyota","label":"Toyota"},{"value":"man","label":"MAN"},{"value":"daf","label":"DAF"},{"value":"scania","label":"Scania"},{"value":"volvo","label":"Volvo"},{"value":"autre","label":"Autre"}]',
     '', '', '', 1, 2, 1, NULL, ''),
    (@StepDetails, 'model', N'Modele', 'dependent_select', N'Choisissez', '', '', '', '', 1, 3, 1, 100, ''),
    (@StepDetails, 'year', N'Annee modele', 'select', N'Choisissez',
     N'[{"value":"2024","label":"2024"},{"value":"2023","label":"2023"},{"value":"2022","label":"2022"},{"value":"2021","label":"2021"},{"value":"2020","label":"2020"},{"value":"2019","label":"2019"},{"value":"2018","label":"2018"},{"value":"2017","label":"2017"},{"value":"2016","label":"2016"},{"value":"2015","label":"2015"},{"value":"avant_2015","label":"Avant 2015"}]',
     '', '', '', 1, 4, 1, NULL, ''),
    (@StepDetails, 'firstCirculation', N'Date de premiere mise en circulation', 'date_month', N'MM/AAAA', '', '', '', N'Mention obligatoire pour vehicule d''occasion.', 0, 5, 1, 7, ''),
    (@StepDetails, 'technicalControl', N'Date de fin de validite du controle technique', 'date_month', N'MM/AAAA', '', '', '', '', 0, 6, 1, 7, ''),
    (@StepDetails, 'fuel', N'Energie', 'pills', N'',
     N'[{"value":"essence","label":"Essence"},{"value":"diesel","label":"Diesel"},{"value":"hybride","label":"Hybride"},{"value":"electrique","label":"Electrique"},{"value":"gpl","label":"GPL"},{"value":"autre","label":"Autre"}]',
     '', '', '', 1, 7, 1, NULL, ''),
    (@StepDetails, 'gearbox', N'Boite de vitesse', 'radio', N'',
     N'[{"value":"manuelle","label":"Manuelle"},{"value":"automatique","label":"Automatique"}]',
     '', '', '', 1, 8, 1, NULL, ''),
    (@StepDetails, 'carrosserie', N'Carrosserie specifique', 'select', N'Choisissez',
     N'[{"value":"militaire","label":"Militaire"},{"value":"betaillere","label":"Betaillere, chevaux"},{"value":"isotherme","label":"Isotherme, frigorifique"},{"value":"evenementiel","label":"Evenementiel"},{"value":"hayon","label":"Hayon elevateur"},{"value":"nacelle","label":"Nacelle, grue"},{"value":"funeraire","label":"Funeraire"},{"value":"food_truck","label":"Commerce, Food Truck"},{"value":"nettoyage","label":"Nettoyage urbain"},{"value":"ambulance","label":"Ambulance, secours, medical"}]',
     '', '', '', 0, 9, 1, NULL, ''),
    (@StepDetails, 'transmission', N'Transmission', 'select', N'Choisissez',
     N'[{"value":"traction","label":"Traction"},{"value":"4x4","label":"4 roues motrices"},{"value":"propulsion","label":"Propulsion"}]',
     '', '', '', 0, 10, 1, NULL, ''),
    (@StepDetails, 'doors', N'Nombre de portes', 'select', N'Choisissez',
     N'[{"value":"2","label":"2"},{"value":"3","label":"3"},{"value":"4","label":"4"},{"value":"5","label":"5"}]',
     '', '', '', 0, 11, 1, NULL, ''),
    (@StepDetails, 'seats', N'Nombre de places', 'select', N'Choisissez',
     N'[{"value":"2","label":"2"},{"value":"3","label":"3"},{"value":"5","label":"5"},{"value":"6","label":"6"},{"value":"7","label":"7"},{"value":"9","label":"9+"}]',
     '', '', '', 0, 12, 1, NULL, ''),
    (@StepDetails, 'fiscalPower', N'Puissance fiscale', 'number', N'0', '', '', 'CV', N'Puissance fiscale de votre vehicule.', 0, 13, 1, NULL, ''),
    (@StepDetails, 'dinPower', N'Puissance DIN', 'number', N'0', '', '', 'Ch', N'Puissance moteur en chevaux DIN.', 0, 14, 1, NULL, ''),
    (@StepDetails, 'volume', N'Volume de chargement', 'select', N'Choisissez',
     N'[{"value":"petit","label":"Petit utilitaire (3 a 5 m3)"},{"value":"moyen","label":"Utilitaire moyen (5 a 9 m3)"},{"value":"grand","label":"Grand utilitaire (9 a 12 m3)"}]',
     '', '', N'Volume utile de l''espace de chargement en metres cubes.', 0, 15, 1, NULL, ''),
    (@StepDetails, 'ptac', N'PTAC', 'select', N'Choisissez',
     N'[{"value":"2_4","label":"<= 2,4 t"},{"value":"2_4_3_5","label":"> 2,4 a 3,5 t"},{"value":"3_5_plus","label":"> 3,5 t"}]',
     '', '', N'Poids Total Autorise en Charge en tonnes.', 0, 16, 1, NULL, ''),
    (@StepDetails, 'finition', N'Finition', 'select', N'Choisissez',
     N'[{"value":"confort","label":"Confort"},{"value":"grand_confort","label":"Grand Confort"},{"value":"business","label":"Business"},{"value":"pack_clim","label":"Pack Clim"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 17, 1, NULL, ''),
    (@StepDetails, 'version', N'Version', 'select', N'Choisissez',
     N'[{"value":"l1h1","label":"L1H1"},{"value":"l1h2","label":"L1H2"},{"value":"l2h1","label":"L2H1"},{"value":"l2h2","label":"L2H2"},{"value":"l3h2","label":"L3H2"},{"value":"l3h3","label":"L3H3"},{"value":"l4h2","label":"L4H2"},{"value":"l4h3","label":"L4H3"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 18, 1, NULL, ''),
    (@StepDetails, 'mileage', N'Kilometrage', 'number', N'0', '', '', 'km', '', 1, 19, 1, NULL, ''),
    (@StepDetails, 'color', N'Couleur', 'select', N'Choisissez',
     N'[{"value":"noir","label":"Noir"},{"value":"blanc","label":"Blanc"},{"value":"gris","label":"Gris"},{"value":"bleu","label":"Bleu"},{"value":"rouge","label":"Rouge"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 20, 1, NULL, ''),
    (@StepDetails, 'tvaRecuperable', N'TVA recuperable', 'radio', N'',
     N'[{"value":"oui","label":"Oui"},{"value":"non","label":"Non"}]',
     '', '', '', 0, 21, 1, NULL, ''),
    (@StepDetails, 'history', N'Historique et entretien', 'multiselect', N'Selectionnez',
     N'[{"value":"premiere_main","label":"Premiere main"},{"value":"carnet_entretien","label":"Carnet d''entretien"},{"value":"revision_jour","label":"Revision a jour"},{"value":"ct_ok","label":"Controle technique OK"},{"value":"import","label":"Vehicule importe"}]',
     '', '', '', 0, 22, 1, NULL, ''),
    (@StepDetails, 'vehicleState', N'Etat du vehicule', 'select', N'Choisissez',
     N'[{"value":"neuf","label":"Neuf"},{"value":"tres_bon","label":"Tres bon etat"},{"value":"bon","label":"Bon etat"},{"value":"correct","label":"Etat correct"},{"value":"accident","label":"Accidente / Pour pieces"}]',
     '', '', '', 0, 23, 1, NULL, ''),
    (@StepDetails, 'critair', N'Crit''Air', 'pills', N'',
     N'[{"value":"0","label":"0"},{"value":"1","label":"1"},{"value":"2","label":"2"},{"value":"3","label":"3"},{"value":"4","label":"4"},{"value":"5","label":"5"},{"value":"nc","label":"Non classe"}]',
     '', '', '', 0, 24, 1, NULL, ''),
    (@StepDetails, 'classeEmission', N'Classe d''emission', 'select', N'Choisissez',
     N'[{"value":"euro1","label":"Euro 1"},{"value":"euro2","label":"Euro 2"},{"value":"euro3","label":"Euro 3"},{"value":"euro4","label":"Euro 4"},{"value":"euro5","label":"Euro 5"},{"value":"euro6","label":"Euro 6"}]',
     '', '', '', 0, 25, 1, NULL, '');

    -- Step: Description
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepDesc, 'description', N'Titre de l''annonce', 'text_counter', N'Ex: Renault Master 2020 L2H2 Fourgon', '', '', '', '', 1, 1, 1, 200, ''),
    (@StepDesc, 'annonce_description', N'Description de l''annonce', 'textarea_counter', N'Decrivez votre vehicule : equipements, entretien, kilometrage...', '', '', '', '', 1, 2, 1, 4000, '');

    -- Step: Price
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepPrice, 'price', N'Votre prix de vente', 'price_gauge', N'Ex: 25000', '', '', 'TND', '', 1, 1, 1, NULL, '');

    -- Step: Location
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepLocation, 'address', N'Adresse', 'address', N'Tapez votre adresse...', '', '', '', N'Completez votre adresse pour etre trouve facilement.', 1, 1, 1, NULL, '');

    -- Step: Contact
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepContact, 'email', N'Email', 'email', N'', '', '', '', '', 1, 1, 1, NULL, ''),
    (@StepContact, 'phone', N'Telephone', 'phone', N'', '', '', '', '', 1, 2, 1, NULL, ''),
    (@StepContact, 'hidePhone', N'Masquer le numero', 'toggle', '', '', 'false', '', '', 0, 3, 1, NULL, '');

    PRINT 'Workflow "Utilitaires" created with 7 steps and 31 fields.';
END
ELSE
    PRINT 'Workflow already exists.';
