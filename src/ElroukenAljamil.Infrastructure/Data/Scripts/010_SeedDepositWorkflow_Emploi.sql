-- ============================================
-- 010: Seed Deposit Workflow for "Offre d'emploi"
-- Data only - assumes tables already exist via EF migration
-- ============================================

DECLARE @CategoryId INT;
SELECT @CategoryId = Id FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Name LIKE N'%Emploi%') AND ShowInDeposit = 1 AND Name LIKE N'%emploi%';

IF @CategoryId IS NULL
BEGIN
    SELECT TOP 1 @CategoryId = Id FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Name LIKE N'%Emploi%') AND ShowInDeposit = 1;
END

-- Only insert if category found and workflow doesn't already exist
IF @CategoryId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM DepositWorkflows WHERE CategoryId = @CategoryId)
BEGIN
    -- Insert Workflow
    INSERT INTO DepositWorkflows (CategoryId, Name, Description, IsActive, CreatedAt)
    VALUES (@CategoryId, N'Depot Offre emploi', N'Workflow complet pour deposer une offre emploi', 1, GETUTCDATE());

    DECLARE @WorkflowId INT = SCOPE_IDENTITY();

    -- Insert Steps
    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired, IsActive) VALUES
    (@WorkflowId, 1, N'Commencons par l''essentiel !', N'* champs obligatoires', 'title', 1, 1),
    (@WorkflowId, 2, N'Ajoutez des photos', N'Vos photos', 'photos', 0, 1),
    (@WorkflowId, 3, N'Dites-nous en plus', N'Ces informations permettront aux candidats de mieux comprendre votre offre.', 'details', 1, 1),
    (@WorkflowId, 4, N'Quel est le niveau de remuneration ?', N'Indiquez le salaire propose pour ce poste.', 'salary', 0, 1),
    (@WorkflowId, 5, N'Decrivez votre recherche !', N'Donnez envie aux recruteurs de vous contacter.', 'description', 1, 1),
    (@WorkflowId, 6, N'Ou vous situez-vous ?', N'Indiquez votre localisation pour etre trouve plus facilement.', 'location', 1, 1),
    (@WorkflowId, 7, N'Vos coordonnees', N'Verifiez vos informations de contact avant publication.', 'contact', 1, 1);

    -- Get Step IDs
    DECLARE @StepTitle INT, @StepPhotos INT, @StepDetails INT, @StepSalary INT, @StepDesc INT, @StepLocation INT, @StepContact INT;
    SELECT @StepTitle = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'title';
    SELECT @StepDetails = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'details';
    SELECT @StepSalary = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'salary';
    SELECT @StepDesc = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'description';
    SELECT @StepLocation = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'location';
    SELECT @StepContact = Id FROM WorkflowSteps WHERE WorkflowId = @WorkflowId AND StepKey = 'contact';

    -- Step: Title (1 field)
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepTitle, 'title', N'Quel est le titre de l''annonce ?', 'text', N'Ex: Developpeur Full Stack Senior', '', '', '', '', 1, 1, 1, 200, '');

    -- Step: Details (6 fields)
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepDetails, 'contract', N'Type de contrat', 'select', N'Choisissez',
     N'[{"value":"cdi","label":"CDI"},{"value":"cdd","label":"CDD"},{"value":"interim","label":"Interim"},{"value":"alternance","label":"Apprentissage / Alternance"},{"value":"stage","label":"Stage"},{"value":"freelance","label":"Independant / Franchise"},{"value":"benevolat","label":"Benevolat"}]',
     '', '', '', 0, 1, 1, NULL, ''),
    (@StepDetails, 'industry', N'Secteur d''activite', 'select', N'Choisissez',
     N'[{"value":"it","label":"Informatique"},{"value":"sales","label":"Commerce / Vente"},{"value":"construction","label":"BTP / Construction"},{"value":"medical","label":"Sante / Medical"},{"value":"logistics","label":"Transport / Logistique"}]',
     '', '', '', 0, 2, 1, NULL, ''),
    (@StepDetails, 'job', N'Metier', 'select', N'Choisissez',
     N'[{"value":"it_digital","label":"Informatique / Digital"},{"value":"commerce","label":"Commerce / Vente / Marketing"},{"value":"sante","label":"Sante / Services a la personne"},{"value":"btp","label":"BTP / Construction / Immobilier"},{"value":"transport","label":"Transport / Logistique"},{"value":"admin","label":"Administration / RH / Juridique"}]',
     '', '', '', 0, 3, 1, NULL, ''),
    (@StepDetails, 'experience', N'Experience', 'select', N'Choisissez',
     N'[{"value":"junior","label":"Junior (0 a 2 ans)"},{"value":"confirme","label":"Confirme (2 a 5 ans)"},{"value":"senior","label":"Senior (5 a 10 ans)"},{"value":"expert","label":"Expert / Lead (+ de 10 ans)"}]',
     '', '', '', 0, 4, 1, NULL, ''),
    (@StepDetails, 'education', N'Niveau d''etudes', 'select', N'Choisissez',
     N'[{"value":"no_degree","label":"Sans diplome"},{"value":"cap_bep","label":"CAP, BEP"},{"value":"bac","label":"Bac, Bac pro, BP"},{"value":"bac_2","label":"Bac +2"},{"value":"bac_3","label":"Bac +3"},{"value":"bac_5","label":"Bac +5 et plus"}]',
     '', '', '', 0, 5, 1, NULL, ''),
    (@StepDetails, 'workType', N'Travail a', 'radio', N'',
     N'[{"value":"temps_plein","label":"Temps plein"},{"value":"temps_partiel","label":"Temps partiel"},{"value":"both","label":"Temps plein ou temps partiel"}]',
     'temps_plein', '', '', 0, 6, 1, NULL, '');

    -- Step: Salary (1 field)
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepSalary, 'salary', N'Salaire horaire', 'number', N'0', '', '', 'TND', N'Precisez le montant brut horaire.', 0, 1, 1, NULL, '');

    -- Step: Description (3 fields)
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepDesc, 'poste', N'Poste recherche', 'text', N'Ex: Developpeur web', '', '', 'H/F',
     N'Vous n''avez pas besoin de mentionner Recherche ou Poste de ici.', 1, 1, 1, 200, ''),
    (@StepDesc, 'experienceDesc', N'Description de vos experiences', 'textarea', N'Decrivez vos competences et experiences...', '', '', '',
     N'La loi interdit toute mention discriminatoire. Seul le travail declare est autorise.', 1, 2, 1, 4000, ''),
    (@StepDesc, 'profilVisible', N'J''autorise la creation de mon profil candidat et sa visibilite aupres des recruteurs.', 'toggle', '', '', 'true', '', '', 0, 3, 1, NULL, '');

    -- Step: Location (1 field)
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepLocation, 'address', N'Adresse', 'address', N'Tapez votre adresse...', '', '', '',
     N'Completez votre adresse et les personnes utilisant la recherche autour de soi trouveront plus facilement votre annonce.', 1, 1, 1, NULL, '');

    -- Step: Contact (3 fields)
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex) VALUES
    (@StepContact, 'email', N'Email', 'email', N'', '', '', '', '', 1, 1, 1, NULL, ''),
    (@StepContact, 'phone', N'Telephone', 'phone', N'', '', '', '', '', 1, 2, 1, NULL, ''),
    (@StepContact, 'hidePhone', N'Masquer le numero', 'toggle', '', '', 'false', '', '', 0, 3, 1, NULL, '');

    PRINT 'Workflow "Offre d''emploi" created successfully with 7 steps and 15 fields.';
END
ELSE
BEGIN
    PRINT 'Workflow already exists or category not found.';
END
