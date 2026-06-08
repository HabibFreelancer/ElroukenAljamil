-- Step 10 (details): 6 fields
INSERT INTO StepFields (StepId,FieldKey,Label,FieldType,Placeholder,Options,DefaultValue,Suffix,HelperText,IsRequired,DisplayOrder,IsActive,MaxLength,ValidationRegex)
VALUES
(10,'contract',N'Type de contrat','select',N'Choisissez',
N'[{"value":"cdi","label":"CDI"},{"value":"cdd","label":"CDD"},{"value":"interim","label":"Interim"},{"value":"alternance","label":"Apprentissage / Alternance"},{"value":"stage","label":"Stage"},{"value":"freelance","label":"Independant / Franchise"},{"value":"benevolat","label":"Benevolat"}]',
'','','',0,1,1,NULL,'');

INSERT INTO StepFields (StepId,FieldKey,Label,FieldType,Placeholder,Options,DefaultValue,Suffix,HelperText,IsRequired,DisplayOrder,IsActive,MaxLength,ValidationRegex)
VALUES
(10,'industry',N'Secteur d''activite','select',N'Choisissez',
N'[{"value":"it","label":"Informatique"},{"value":"sales","label":"Commerce / Vente"},{"value":"construction","label":"BTP / Construction"},{"value":"medical","label":"Sante / Medical"},{"value":"logistics","label":"Transport / Logistique"}]',
'','','',0,2,1,NULL,'');

INSERT INTO StepFields (StepId,FieldKey,Label,FieldType,Placeholder,Options,DefaultValue,Suffix,HelperText,IsRequired,DisplayOrder,IsActive,MaxLength,ValidationRegex)
VALUES
(10,'job',N'Metier','select',N'Choisissez',
N'[{"value":"it_digital","label":"Informatique / Digital"},{"value":"commerce","label":"Commerce / Vente / Marketing"},{"value":"sante","label":"Sante / Services a la personne"},{"value":"btp","label":"BTP / Construction / Immobilier"},{"value":"transport","label":"Transport / Logistique"},{"value":"admin","label":"Administration / RH / Juridique"}]',
'','','',0,3,1,NULL,'');

INSERT INTO StepFields (StepId,FieldKey,Label,FieldType,Placeholder,Options,DefaultValue,Suffix,HelperText,IsRequired,DisplayOrder,IsActive,MaxLength,ValidationRegex)
VALUES
(10,'experience',N'Experience','select',N'Choisissez',
N'[{"value":"junior","label":"Junior (0 a 2 ans)"},{"value":"confirme","label":"Confirme (2 a 5 ans)"},{"value":"senior","label":"Senior (5 a 10 ans)"},{"value":"expert","label":"Expert / Lead (+ de 10 ans)"}]',
'','','',0,4,1,NULL,'');

INSERT INTO StepFields (StepId,FieldKey,Label,FieldType,Placeholder,Options,DefaultValue,Suffix,HelperText,IsRequired,DisplayOrder,IsActive,MaxLength,ValidationRegex)
VALUES
(10,'education',N'Niveau d''etudes','select',N'Choisissez',
N'[{"value":"no_degree","label":"Sans diplome"},{"value":"cap_bep","label":"CAP, BEP"},{"value":"bac","label":"Bac, Bac pro, BP"},{"value":"bac_2","label":"Bac +2"},{"value":"bac_3","label":"Bac +3"},{"value":"bac_5","label":"Bac +5 et plus"}]',
'','','',0,5,1,NULL,'');

INSERT INTO StepFields (StepId,FieldKey,Label,FieldType,Placeholder,Options,DefaultValue,Suffix,HelperText,IsRequired,DisplayOrder,IsActive,MaxLength,ValidationRegex)
VALUES
(10,'workType',N'Travail a','radio',N'',
N'[{"value":"temps_plein","label":"Temps plein"},{"value":"temps_partiel","label":"Temps partiel"},{"value":"both","label":"Temps plein ou temps partiel"}]',
'temps_plein','','',0,6,1,NULL,'');

-- Step 11 (salary): 1 field
INSERT INTO StepFields (StepId,FieldKey,Label,FieldType,Placeholder,Options,DefaultValue,Suffix,HelperText,IsRequired,DisplayOrder,IsActive,MaxLength,ValidationRegex)
VALUES
(11,'salary',N'Salaire horaire','number',N'0','','','TND',N'Precisez le montant brut horaire.',0,1,1,NULL,'');

-- Step 12 (description): 3 fields
INSERT INTO StepFields (StepId,FieldKey,Label,FieldType,Placeholder,Options,DefaultValue,Suffix,HelperText,IsRequired,DisplayOrder,IsActive,MaxLength,ValidationRegex)
VALUES
(12,'poste',N'Poste recherche','text',N'Ex: Developpeur web','','','H/F',
N'Vous n''avez pas besoin de mentionner Recherche ou Poste de ici.',1,1,1,200,'');

INSERT INTO StepFields (StepId,FieldKey,Label,FieldType,Placeholder,Options,DefaultValue,Suffix,HelperText,IsRequired,DisplayOrder,IsActive,MaxLength,ValidationRegex)
VALUES
(12,'experienceDesc',N'Description de vos experiences','textarea',N'Decrivez vos competences et experiences...','','','',
N'La loi interdit toute mention discriminatoire. Seul le travail declare est autorise.',1,2,1,4000,'');

INSERT INTO StepFields (StepId,FieldKey,Label,FieldType,Placeholder,Options,DefaultValue,Suffix,HelperText,IsRequired,DisplayOrder,IsActive,MaxLength,ValidationRegex)
VALUES
(12,'profilVisible',N'J''autorise la creation de mon profil candidat et sa visibilite aupres des recruteurs.','toggle','','','true','','',0,3,1,NULL,'');

-- Step 13 (location): 1 field
INSERT INTO StepFields (StepId,FieldKey,Label,FieldType,Placeholder,Options,DefaultValue,Suffix,HelperText,IsRequired,DisplayOrder,IsActive,MaxLength,ValidationRegex)
VALUES
(13,'address',N'Adresse','address',N'Tapez votre adresse...','','','',
N'Completez votre adresse et les personnes utilisant la recherche autour de soi trouveront plus facilement votre annonce.',1,1,1,NULL,'');

-- Step 14 (contact): 3 fields
INSERT INTO StepFields (StepId,FieldKey,Label,FieldType,Placeholder,Options,DefaultValue,Suffix,HelperText,IsRequired,DisplayOrder,IsActive,MaxLength,ValidationRegex)
VALUES
(14,'email',N'Email','email',N'','','','','',1,1,1,NULL,'');

INSERT INTO StepFields (StepId,FieldKey,Label,FieldType,Placeholder,Options,DefaultValue,Suffix,HelperText,IsRequired,DisplayOrder,IsActive,MaxLength,ValidationRegex)
VALUES
(14,'phone',N'Telephone','phone',N'','','','','',1,2,1,NULL,'');

INSERT INTO StepFields (StepId,FieldKey,Label,FieldType,Placeholder,Options,DefaultValue,Suffix,HelperText,IsRequired,DisplayOrder,IsActive,MaxLength,ValidationRegex)
VALUES
(14,'hidePhone',N'Masquer le numero','toggle','','','false','','',0,3,1,NULL,'');

PRINT 'All fields inserted successfully.';
