-- ============================================
-- 011: Seed Deposit Workflow for "Véhicules > Voitures"
-- Data only - assumes tables already exist via EF migration
-- ============================================

DECLARE @CategoryId INT;
SELECT @CategoryId = Id FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Name LIKE N'%hicule%') AND ShowInDeposit = 1 AND Name LIKE N'%oiture%';

IF @CategoryId IS NULL
BEGIN
    SELECT TOP 1 @CategoryId = Id FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Name LIKE N'%hicule%') AND ShowInDeposit = 1;
END

IF @CategoryId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM DepositWorkflows WHERE CategoryId = @CategoryId)
BEGIN
    INSERT INTO DepositWorkflows (CategoryId, Name, Description, IsActive, CreatedAt)
    VALUES (@CategoryId, N'Depot Vehicules Voitures', N'Workflow complet pour deposer une annonce vehicule', 1, GETUTCDATE());

    DECLARE @WorkflowId INT = SCOPE_IDENTITY();

    -- Insert Steps
    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired, IsActive) VALUES
    (@WorkflowId, 1, N'Commencons par l''essentiel !', N'* champs obligatoires', 'title', 1, 1),
    (@WorkflowId, 2, N'Ajoutez des photos', N'Faites glisser vos photos pour changer leur ordre', 'photos', 0, 1),
    (@WorkflowId, 3, N'Dites-nous en plus', N'L''immatriculation de votre vehicule est obligatoire pour continuer.', 'details', 1, 1),
    (@WorkflowId, 4, N'Decrivez votre bien !', N'Une bonne description augmente vos chances de vendre rapidement.', 'description', 1, 1),
    (@WorkflowId, 5, N'Ou se situe le vehicule ?', N'Indiquez la localisation pour etre trouve facilement.', 'location', 1, 1),
    (@WorkflowId, 6, N'Vos coordonnees', N'Verifiez vos informations de contact avant publication.', 'contact', 1, 1);

    DECLARE @StepTitle INT, @StepDetails INT, @StepDesc INT, @StepLocation INT, @StepContact INT;
    SELECT @StepTitle = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'title';
    SELECT @StepDetails = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'details';
    SELECT @StepDesc = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'description';
    SELECT @StepLocation = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'location';
    SELECT @StepContact = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'contact';

    -- Step: Title
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepTitle, 'title', N'Titre de l''annonce', 'text', N'Ex: Peugeot 308 2019 Diesel 85000km', '', '', '', '', 1, 1, 1, 200, '');

    -- Step: Details (vehicule specifics)
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepDetails, 'immatriculation', N'Numero d''immatriculation', 'immatriculation', N'Ex: 123 TU 4567', '', '', '',
     N'Il ne sera pas visible sur l''annonce. Il est conserve pour pre-remplir d''autres formulaires.', 1, 1, 1, 20, ''),
    (@StepDetails, 'brand', N'Marque', 'select', N'Choisissez',
     N'[{"value":"peugeot","label":"Peugeot"},{"value":"renault","label":"Renault"},{"value":"citroen","label":"Citroen"},{"value":"volkswagen","label":"Volkswagen"},{"value":"bmw","label":"BMW"},{"value":"mercedes","label":"Mercedes"},{"value":"audi","label":"Audi"},{"value":"toyota","label":"Toyota"},{"value":"hyundai","label":"Hyundai"},{"value":"kia","label":"Kia"},{"value":"fiat","label":"Fiat"},{"value":"nissan","label":"Nissan"},{"value":"ford","label":"Ford"},{"value":"opel","label":"Opel"},{"value":"dacia","label":"Dacia"},{"value":"seat","label":"Seat"},{"value":"skoda","label":"Skoda"},{"value":"suzuki","label":"Suzuki"},{"value":"autre","label":"Autre"}]',
     '', '', '', 1, 2, 1, NULL, ''),
    (@StepDetails, 'model', N'Modele', 'dependent_select', N'Choisissez', '', '', '', '', 1, 3, 1, 100, ''),
    (@StepDetails, 'year', N'Annee modele', 'select', N'Choisissez',
     N'[{"value":"2024","label":"2024"},{"value":"2023","label":"2023"},{"value":"2022","label":"2022"},{"value":"2021","label":"2021"},{"value":"2020","label":"2020"},{"value":"2019","label":"2019"},{"value":"2018","label":"2018"},{"value":"2017","label":"2017"},{"value":"2016","label":"2016"},{"value":"2015","label":"2015"},{"value":"2014","label":"2014"},{"value":"2013","label":"2013"},{"value":"2012","label":"2012"},{"value":"2011","label":"2011"},{"value":"2010","label":"2010"},{"value":"avant_2010","label":"Avant 2010"}]',
     '', '', '', 1, 4, 1, NULL, ''),
    (@StepDetails, 'firstCirculation', N'Date de premiere mise en circulation', 'date_month', N'MM/AAAA', '', '', '',
     N'Mention obligatoire dans le cadre de la vente de vehicule d''occasion.', 0, 5, 1, 7, ''),
    (@StepDetails, 'technicalControl', N'Date de fin de validite du controle technique', 'date_month', N'MM/AAAA', '', '', '', '', 0, 6, 1, 7, ''),
    (@StepDetails, 'fuel', N'Energie', 'pills', N'',
     N'[{"value":"essence","label":"Essence"},{"value":"diesel","label":"Diesel"},{"value":"hybride","label":"Hybride"},{"value":"hybride_rechargeable","label":"Hybride Rechargeable"},{"value":"electrique","label":"Electrique"},{"value":"gpl","label":"GPL"},{"value":"gnv","label":"Gaz Naturel (GNV)"},{"value":"autre","label":"Autre"}]',
     '', '', '', 1, 7, 1, NULL, ''),
    (@StepDetails, 'gearbox', N'Boite de vitesse', 'radio', N'',
     N'[{"value":"manuelle","label":"Manuelle"},{"value":"automatique","label":"Automatique"}]',
     '', '', '', 1, 8, 1, NULL, ''),
    (@StepDetails, 'vehicleType', N'Type de vehicule', 'select', N'Choisissez',
     N'[{"value":"berline","label":"Berline"},{"value":"break","label":"Break"},{"value":"cabriolet","label":"Cabriolet"},{"value":"citadine","label":"Citadine"},{"value":"coupe","label":"Coupe"},{"value":"monospace","label":"Monospace"},{"value":"suv","label":"4x4, SUV & Crossover"},{"value":"utilitaire","label":"Utilitaire"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 9, 1, NULL, ''),
    (@StepDetails, 'doors', N'Nombre de portes', 'select', N'Choisissez',
     N'[{"value":"2","label":"2"},{"value":"3","label":"3"},{"value":"4","label":"4"},{"value":"5","label":"5"}]',
     '', '', '', 0, 10, 1, NULL, ''),
    (@StepDetails, 'seats', N'Nombre de places', 'select', N'Choisissez',
     N'[{"value":"2","label":"2"},{"value":"4","label":"4"},{"value":"5","label":"5"},{"value":"6","label":"6"},{"value":"7","label":"7"},{"value":"8","label":"8"},{"value":"9","label":"9+"}]',
     '', '', '', 0, 11, 1, NULL, ''),
    (@StepDetails, 'fiscalPower', N'Puissance fiscale', 'number', N'0', '', '', 'CV',
     N'Il s''agit de la puissance fiscale de votre vehicule.', 0, 12, 1, NULL, ''),
    (@StepDetails, 'dinPower', N'Puissance DIN', 'number', N'0', '', '', 'Ch',
     N'Il s''agit de la puissance moteur (exprimee en chevaux DIN ou Kw).', 0, 13, 1, NULL, ''),
    (@StepDetails, 'mileage', N'Kilometrage', 'number', N'0', '', '', 'km', '', 1, 14, 1, NULL, ''),
    (@StepDetails, 'color', N'Couleur', 'select', N'Choisissez',
     N'[{"value":"noir","label":"Noir"},{"value":"blanc","label":"Blanc"},{"value":"gris","label":"Gris"},{"value":"bleu","label":"Bleu"},{"value":"rouge","label":"Rouge"},{"value":"vert","label":"Vert"},{"value":"beige","label":"Beige"},{"value":"marron","label":"Marron"},{"value":"orange","label":"Orange"},{"value":"jaune","label":"Jaune"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 15, 1, NULL, ''),
    (@StepDetails, 'upholstery', N'Sellerie', 'multiselect', N'Selectionnez',
     N'[{"value":"tissu","label":"Tissu"},{"value":"cuir","label":"Cuir"},{"value":"alcantara","label":"Alcantara"},{"value":"semi_cuir","label":"Semi-cuir"},{"value":"simili","label":"Simili cuir"},{"value":"velours","label":"Velours"},{"value":"autre","label":"Autre"}]',
     '', '', '', 0, 16, 1, NULL, ''),
    (@StepDetails, 'equipment', N'Equipements', 'multiselect', N'Selectionnez',
     N'[{"value":"climatisation","label":"Climatisation"},{"value":"gps","label":"GPS"},{"value":"bluetooth","label":"Bluetooth"},{"value":"camera_recul","label":"Camera de recul"},{"value":"radar_stationnement","label":"Radar de stationnement"},{"value":"siege_chauffant","label":"Sieges chauffants"},{"value":"toit_ouvrant","label":"Toit ouvrant"},{"value":"regulateur","label":"Regulateur de vitesse"},{"value":"start_stop","label":"Start & Stop"},{"value":"jantes_alu","label":"Jantes aluminium"},{"value":"led","label":"Phares LED"},{"value":"keyless","label":"Demarrage sans cle"},{"value":"apple_carplay","label":"Apple CarPlay"},{"value":"android_auto","label":"Android Auto"},{"value":"aide_stationnement","label":"Aide au stationnement"},{"value":"volant_cuir","label":"Volant cuir"}]',
     '', '', '', 0, 17, 1, NULL, ''),
    (@StepDetails, 'history', N'Historique et entretien', 'multiselect', N'Selectionnez',
     N'[{"value":"premiere_main","label":"Premiere main"},{"value":"carnet_entretien","label":"Carnet d''entretien"},{"value":"revision_jour","label":"Revision a jour"},{"value":"non_fumeur","label":"Non fumeur"},{"value":"garage","label":"Gare en garage"},{"value":"ct_ok","label":"Controle technique OK"},{"value":"import","label":"Vehicule importe"},{"value":"accident","label":"Vehicule accidente"}]',
     '', '', '', 0, 18, 1, NULL, ''),
    (@StepDetails, 'vehicleState', N'Etat du vehicule', 'select', N'Choisissez',
     N'[{"value":"neuf","label":"Neuf"},{"value":"tres_bon","label":"Tres bon etat"},{"value":"bon","label":"Bon etat"},{"value":"correct","label":"Etat correct"},{"value":"accident","label":"Accidente / Pour pieces"}]',
     '', '', '', 0, 19, 1, NULL, ''),
    (@StepDetails, 'license', N'Permis', 'radio', N'',
     N'[{"value":"avec","label":"Avec permis"},{"value":"sans","label":"Sans permis"}]',
     'avec', '', '', 0, 20, 1, NULL, '');

    -- Step: Description (titre + description IA)
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepDesc, 'description', N'Titre de l''annonce', 'text_counter', N'Ex: Peugeot 308 2019 Diesel 85000km', '', '', '', '', 1, 1, 1, 200, ''),
    (@StepDesc, 'annonce_description', N'Description de l''annonce', 'textarea_counter', N'Decrivez votre vehicule : equipements, entretien, historique...', '', '', '', '', 1, 2, 1, 4000, '');

    -- Step: Location
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepLocation, 'address', N'Adresse', 'address', N'Tapez votre adresse...', '', '', '',
     N'Completez votre adresse pour etre trouve facilement.', 1, 1, 1, NULL, '');

    -- Step: Contact
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepContact, 'email', N'Email', 'email', N'', '', '', '', '', 1, 1, 1, NULL, ''),
    (@StepContact, 'phone', N'Telephone', 'phone', N'', '', '', '', '', 1, 2, 1, NULL, ''),
    (@StepContact, 'hidePhone', N'Masquer le numero', 'toggle', '', '', 'false', '', '', 0, 3, 1, NULL, '');

    PRINT 'Workflow "Vehicules > Voitures" created with 6 steps and 25 fields.';
END
ELSE
    PRINT 'Workflow already exists or category not found.';
