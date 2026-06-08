-- ============================================
-- 010: Deposit Workflow Configuration
-- Creates tables and seeds workflow for "Offre d'emploi"
-- ============================================

-- Create DepositWorkflows table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DepositWorkflows')
BEGIN
    CREATE TABLE DepositWorkflows (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CategoryId INT NOT NULL,
        Name NVARCHAR(200) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_DepositWorkflows_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE
    );
END

-- Create WorkflowSteps table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WorkflowSteps')
BEGIN
    CREATE TABLE WorkflowSteps (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        WorkflowId INT NOT NULL,
        StepOrder INT NOT NULL,
        Title NVARCHAR(200) NOT NULL,
        Subtitle NVARCHAR(500) NULL,
        StepKey NVARCHAR(50) NOT NULL,
        IsRequired BIT NOT NULL DEFAULT 1,
        IsActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK_WorkflowSteps_Workflows FOREIGN KEY (WorkflowId) REFERENCES DepositWorkflows(Id) ON DELETE CASCADE
    );
END

-- Create StepFields table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StepFields')
BEGIN
    CREATE TABLE StepFields (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        StepId INT NOT NULL,
        FieldKey NVARCHAR(100) NOT NULL,
        Label NVARCHAR(200) NOT NULL,
        FieldType NVARCHAR(50) NOT NULL,
        Placeholder NVARCHAR(500) NULL,
        Options NVARCHAR(MAX) NULL,
        DefaultValue NVARCHAR(500) NULL,
        Suffix NVARCHAR(50) NULL,
        HelperText NVARCHAR(500) NULL,
        IsRequired BIT NOT NULL DEFAULT 0,
        DisplayOrder INT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        MaxLength INT NULL,
        ValidationRegex NVARCHAR(500) NULL,
        CONSTRAINT FK_StepFields_Steps FOREIGN KEY (StepId) REFERENCES WorkflowSteps(Id) ON DELETE CASCADE
    );
END

-- ============================================
-- Seed: Workflow for "Offre d'emploi"
-- Assumes category "Offres d'emploi" exists under Emploi menu
-- ============================================

DECLARE @CategoryId INT;
SELECT @CategoryId = Id FROM Categories WHERE Name LIKE '%Offres d''emploi%' OR (Name LIKE '%emploi%' AND ShowInDeposit = 1);

-- If not found, try the Emploi menu's first ShowInDeposit category
IF @CategoryId IS NULL
BEGIN
    DECLARE @EmploiMenuId INT;
    SELECT @EmploiMenuId = Id FROM Menus WHERE Name LIKE '%Emploi%';
    SELECT TOP 1 @CategoryId = Id FROM Categories WHERE MenuId = @EmploiMenuId AND ShowInDeposit = 1;
END

-- Only insert if category found and workflow doesn't already exist
IF @CategoryId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM DepositWorkflows WHERE CategoryId = @CategoryId)
BEGIN
    -- Insert Workflow
    INSERT INTO DepositWorkflows (CategoryId, Name, Description, IsActive)
    VALUES (@CategoryId, N'Dépôt Offre d''emploi', N'Workflow complet pour déposer une offre d''emploi', 1);

    DECLARE @WorkflowId INT = SCOPE_IDENTITY();

    -- ===== Step 1: Titre & Catégorie =====
    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired)
    VALUES (@WorkflowId, 1, N'Commençons par l''essentiel !', N'* champs obligatoires', 'title', 1);

    DECLARE @Step1Id INT = SCOPE_IDENTITY();

    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, IsRequired, DisplayOrder, MaxLength)
    VALUES (@Step1Id, 'title', N'Quel est le titre de l''annonce ?', 'text', N'Ex: Développeur Full Stack Senior', 1, 1, 200);

    -- ===== Step 2: Photos =====
    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired)
    VALUES (@WorkflowId, 2, N'Ajoutez des photos', N'Vos photos', 'photos', 0);

    -- ===== Step 3: Dites-nous en plus =====
    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired)
    VALUES (@WorkflowId, 3, N'Dites-nous en plus', N'Ces informations permettront aux candidats de mieux comprendre votre offre.', 'details', 1);

    DECLARE @Step3Id INT = SCOPE_IDENTITY();

    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, IsRequired, DisplayOrder) VALUES
    (@Step3Id, 'contract', N'Type de contrat', 'select', N'Choisissez',
     N'[{"value":"cdi","label":"CDI"},{"value":"cdd","label":"CDD"},{"value":"interim","label":"Intérim"},{"value":"alternance","label":"Apprentissage / Alternance"},{"value":"stage","label":"Stage"},{"value":"freelance","label":"Indépendant / Franchise"},{"value":"benevolat","label":"Bénévolat"}]',
     0, 1),
    (@Step3Id, 'industry', N'Secteur d''activité', 'select', N'Choisissez',
     N'[{"value":"it","label":"Informatique"},{"value":"sales","label":"Commerce / Vente"},{"value":"construction","label":"BTP / Construction"},{"value":"medical","label":"Santé / Médical"},{"value":"logistics","label":"Transport / Logistique"},{"value":"finance","label":"Banque / Finance"},{"value":"education","label":"Enseignement / Formation"},{"value":"agriculture","label":"Agriculture / Environnement"}]',
     0, 2),
    (@Step3Id, 'job', N'Métier', 'select', N'Choisissez',
     N'[{"value":"it_digital","label":"Informatique / Digital"},{"value":"commerce","label":"Commerce / Vente / Marketing"},{"value":"sante","label":"Santé / Services à la personne"},{"value":"btp","label":"BTP / Construction / Immobilier"},{"value":"hotellerie","label":"Hôtellerie / Restauration / Tourisme"},{"value":"transport","label":"Transport / Logistique"},{"value":"admin","label":"Administration / RH / Juridique"},{"value":"enseignement","label":"Enseignement / Formation"},{"value":"artisanat","label":"Artisanat / Industrie"},{"value":"finance","label":"Banque / Finance / Comptabilité"},{"value":"agriculture","label":"Agriculture / Environnement"}]',
     0, 3),
    (@Step3Id, 'experience', N'Expérience', 'select', N'Choisissez',
     N'[{"value":"junior","label":"Junior (0 à 2 ans)"},{"value":"confirme","label":"Confirmé (2 à 5 ans)"},{"value":"senior","label":"Sénior (5 à 10 ans)"},{"value":"expert","label":"Expert / Lead (+ de 10 ans)"}]',
     0, 4),
    (@Step3Id, 'education', N'Niveau d''études', 'select', N'Choisissez',
     N'[{"value":"no_degree","label":"Sans diplôme"},{"value":"cap_bep","label":"CAP, BEP"},{"value":"bac","label":"Bac, Bac pro, BP"},{"value":"bac_2","label":"Bac +2"},{"value":"bac_3","label":"Bac +3"},{"value":"bac_5","label":"Bac +5 et plus"}]',
     0, 5),
    (@Step3Id, 'workType', N'Travail à', 'radio', N'',
     N'[{"value":"temps_plein","label":"Temps plein"},{"value":"temps_partiel","label":"Temps partiel"},{"value":"both","label":"Temps plein ou temps partiel"}]',
     0, 6);

    -- ===== Step 4: Rémunération =====
    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired)
    VALUES (@WorkflowId, 4, N'Quel est le niveau de rémunération ?', N'Indiquez le salaire proposé pour ce poste.', 'salary', 0);

    DECLARE @Step4Id INT = SCOPE_IDENTITY();

    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Suffix, HelperText, IsRequired, DisplayOrder)
    VALUES (@Step4Id, 'salary', N'Salaire horaire', 'number', N'0', N'TND', N'Précisez le montant brut horaire.', 0, 1);

    -- ===== Step 5: Décrivez votre recherche =====
    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired)
    VALUES (@WorkflowId, 5, N'Décrivez votre recherche !', N'Donnez envie aux recruteurs de vous contacter.', 'description', 1);

    DECLARE @Step5Id INT = SCOPE_IDENTITY();

    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Suffix, HelperText, IsRequired, DisplayOrder, MaxLength) VALUES
    (@Step5Id, 'poste', N'Poste recherché', 'text', N'Ex: Développeur web', N'H/F',
     N'Vous n''avez pas besoin de mentionner « Recherche » ou « Poste de » ici.', 1, 1, 200),
    (@Step5Id, 'experienceDesc', N'Description de vos expériences', 'textarea', N'Décrivez vos compétences et expériences...', N'',
     N'La loi interdit toute mention discriminatoire. Seul le travail déclaré est autorisé.', 1, 2, 4000);

    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, DefaultValue, HelperText, IsRequired, DisplayOrder)
    VALUES (@Step5Id, 'profilVisible', N'J''autorise la création de mon profil candidat et sa visibilité auprès des recruteurs.', 'toggle', 'true', N'', 0, 3);

    -- ===== Step 6: Localisation =====
    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired)
    VALUES (@WorkflowId, 6, N'Où vous situez-vous ?', N'Indiquez votre localisation pour être trouvé plus facilement.', 'location', 1);

    DECLARE @Step6Id INT = SCOPE_IDENTITY();

    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, HelperText, IsRequired, DisplayOrder)
    VALUES (@Step6Id, 'address', N'Adresse', 'address', N'Tapez votre adresse...',
     N'Complétez votre adresse et les personnes utilisant la recherche autour de soi trouveront plus facilement votre annonce.', 1, 1);

    -- ===== Step 7: Vos coordonnées =====
    INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired)
    VALUES (@WorkflowId, 7, N'Vos coordonnées', N'Vérifiez vos informations de contact avant publication.', 'contact', 1);

    DECLARE @Step7Id INT = SCOPE_IDENTITY();

    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, IsRequired, DisplayOrder) VALUES
    (@Step7Id, 'email', N'Email', 'email', N'', 1, 1),
    (@Step7Id, 'phone', N'Téléphone', 'phone', N'', 1, 2);

    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, DefaultValue, IsRequired, DisplayOrder)
    VALUES (@Step7Id, 'hidePhone', N'Masquer le numéro', 'toggle', 'false', 0, 3);

    PRINT 'Workflow "Offre d''emploi" created successfully with 7 steps.';
END
ELSE
BEGIN
    PRINT 'Workflow already exists or category not found.';
END
