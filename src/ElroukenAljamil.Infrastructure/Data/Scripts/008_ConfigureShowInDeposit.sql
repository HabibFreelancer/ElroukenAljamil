-- ============================================
-- Configurer ShowInDeposit pour les catégories
-- Réinitialiser tout à 0, puis activer seulement les catégories voulues
-- ============================================

-- Réinitialiser tout à 0
UPDATE Categories SET ShowInDeposit = 0;

-- ============================================
-- IMMOBILIER
-- ============================================
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'immobilier') 
AND ParentCategoryId IS NULL
AND Name IN (N'Ventes immobilières', N'Locations', N'Colocations', N'Bureau & Commerce');

-- ============================================
-- VÉHICULES
-- ============================================
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'vehicules') 
AND ParentCategoryId IS NULL
AND Name IN (N'Voitures', N'Motos', N'Caravaning', N'Utilitaires', N'Nautisme', 
             N'Équipement auto', N'Équipement moto', N'Équipement caravaning', N'Équipement nautisme');

-- ============================================
-- VACANCES (Locations de vacances)
-- ============================================
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'vacances') 
AND ParentCategoryId IS NULL
AND Name = N'Types d''hébergements';

-- ============================================
-- EMPLOI
-- ============================================
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'emploi') 
AND ParentCategoryId IS NULL
AND Name IN (N'Offres d''emploi', N'Formations professionnelles');

-- ============================================
-- MODE
-- ============================================
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'mode') 
AND ParentCategoryId IS NULL
AND Name IN (N'Vêtements', N'Chaussures', N'Accessoires & Bagagerie', N'Montres & Bijoux');

-- ============================================
-- MAISON & JARDIN
-- ============================================
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'maison-jardin') 
AND ParentCategoryId IS NULL
AND Name IN (N'Ameublement', N'Papeterie & Fournitures scolaires', N'Électroménager', 
             N'Arts de la table', N'Décoration', N'Linge de maison', N'Bricolage', N'Jardin & Plantes');

-- ============================================
-- FAMILLE
-- ============================================
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'famille') 
AND ParentCategoryId IS NULL
AND Name IN (N'Équipement bébé', N'Mobilier enfant', N'Vêtements bébé');

-- ============================================
-- ÉLECTRONIQUE
-- ============================================
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'electronique') 
AND ParentCategoryId IS NULL
AND Name IN (N'Ordinateurs', N'Accessoires informatique', N'Tablettes & Liseuses', 
             N'Photo, audio & vidéo', N'Téléphones & Objets connectés', 
             N'Accessoires téléphone & Objets connectés', N'Consoles', N'Jeux vidéo');

-- ============================================
-- LOISIRS
-- ============================================
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'loisirs') 
AND ParentCategoryId IS NULL
AND Name IN (N'Antiquités', N'Collection', N'CD - Musique', N'DVD - Films', 
             N'Instruments de musique', N'Livres', N'Modélisme', N'Vins & Gastronomie', 
             N'Jeux & Jouets', N'Loisirs créatifs', N'Sport & Plein air', N'Vélos', N'Équipements vélos');

-- ============================================
-- AUTRES (Animaux, Matériel professionnel, Services, Divers)
-- ============================================

-- Animaux (sous-catégories de la catégorie "Animaux" du menu Autres)
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'autre')
AND ParentCategoryId = (SELECT Id FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'autre') AND Name = N'Animaux' AND ParentCategoryId IS NULL)
AND Name IN (N'Animaux', N'Accessoires animaux', N'Animaux perdus');

-- Aussi activer la catégorie racine Animaux
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'autre') 
AND ParentCategoryId IS NULL
AND Name = N'Animaux';

-- Matériel professionnel (sous-catégories)
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'autre')
AND ParentCategoryId = (SELECT Id FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'autre') AND Name = N'Matériel professionnel' AND ParentCategoryId IS NULL)
AND Name IN (N'Tracteurs', N'Matériel agricole', N'BTP - Chantier gros-oeuvre', N'Poids lourds', 
             N'Manutention - Levage', N'Équipements industriels', N'Équipements pour restaurants & hôtels', 
             N'Équipements & Fournitures de bureau', N'Équipements pour commerces & marchés', N'Matériel médical');

-- Aussi activer la catégorie racine Matériel professionnel
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'autre') 
AND ParentCategoryId IS NULL
AND Name = N'Matériel professionnel';

-- Services (sous-catégories)
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'autre')
AND ParentCategoryId = (SELECT Id FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'autre') AND Name = N'Services' AND ParentCategoryId IS NULL)
AND Name IN (N'Artistes & Musiciens', N'Baby-Sitting', N'Billetterie', N'Covoiturage', 
             N'Cours particuliers', N'Entraide entre voisins', N'Évènements', N'Services à la personne', 
             N'Services aux animaux', N'Services de déménagement', N'Services de réparations électroniques', 
             N'Services de jardinerie & bricolage', N'Services évènementiels', N'Autres services');

-- Aussi activer la catégorie racine Services
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'autre') 
AND ParentCategoryId IS NULL
AND Name = N'Services';

-- Divers (catégorie racine "Autres")
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'autre') 
AND ParentCategoryId IS NULL
AND Name = N'Autres';
