-- ============================================
-- Supprimer et réinsérer les catégories du menu Autres
-- ============================================

-- Supprimer d'abord les sous-catégories (enfants), puis les catégories racines
DELETE FROM Categories WHERE ParentCategoryId IN (SELECT Id FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'autre'));
DELETE FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'autre');

-- Insérer les catégories racines
DECLARE @menuId INT = (SELECT Id FROM Menus WHERE Slug = 'autre');

INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, NULL, N'Matériel professionnel', 'materiel-professionnel', 1, 1, 1),
(@menuId, NULL, N'Services', 'services', 1, 2, 1),
(@menuId, NULL, N'Animaux', 'animaux', 1, 3, 1),
(@menuId, NULL, N'Dons', 'dons', 1, 4, 1),
(@menuId, NULL, N'Autres', 'autres', 1, 5, 1);

-- Sous-catégories Matériel professionnel
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Matériel professionnel' AND ParentCategoryId IS NULL), N'Tracteurs', 'materiel-professionnel/tracteurs', 1, 1, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Matériel professionnel' AND ParentCategoryId IS NULL), N'Matériel agricole', 'materiel-professionnel/agricole', 1, 2, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Matériel professionnel' AND ParentCategoryId IS NULL), N'BTP - Chantier gros-oeuvre', 'materiel-professionnel/btp', 1, 3, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Matériel professionnel' AND ParentCategoryId IS NULL), N'Poids lourds', 'materiel-professionnel/poids-lourds', 1, 4, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Matériel professionnel' AND ParentCategoryId IS NULL), N'Manutention - Levage', 'materiel-professionnel/manutention', 1, 5, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Matériel professionnel' AND ParentCategoryId IS NULL), N'Équipements industriels', 'materiel-professionnel/industriels', 1, 6, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Matériel professionnel' AND ParentCategoryId IS NULL), N'Équipements pour restaurants & hôtels', 'materiel-professionnel/restaurants', 1, 7, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Matériel professionnel' AND ParentCategoryId IS NULL), N'Équipements & Fournitures de bureau', 'materiel-professionnel/bureau', 1, 8, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Matériel professionnel' AND ParentCategoryId IS NULL), N'Équipements pour commerces & marchés', 'materiel-professionnel/commerces', 1, 9, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Matériel professionnel' AND ParentCategoryId IS NULL), N'Matériel médical', 'materiel-professionnel/medical', 1, 10, 1);

-- Sous-catégories Services
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Services de déménagement', 'services/demenagement', 1, 1, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Services de réparations mécaniques', 'services/reparations-mecaniques', 1, 2, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Services de jardinerie & bricolage', 'services/jardinerie-bricolage', 1, 3, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Services à la personne', 'services/personne', 1, 4, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Services aux animaux', 'services/animaux', 1, 5, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Baby-Sitting', 'services/baby-sitting', 1, 6, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Artistes & Musiciens', 'services/artistes-musiciens', 1, 7, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Services évènementiels', 'services/evenementiels', 1, 8, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Services de réparations électroniques', 'services/reparations-electroniques', 1, 9, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Entraide entre voisins', 'services/entraide-voisins', 1, 10, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Billetterie', 'services/billetterie', 1, 11, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Évènements', 'services/evenements', 1, 12, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Covoiturage', 'services/covoiturage', 1, 13, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Cours particuliers', 'services/cours-particuliers', 1, 14, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Services' AND ParentCategoryId IS NULL), N'Autres services', 'services/autres', 1, 15, 1);

-- Sous-catégories Animaux
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Animaux' AND ParentCategoryId IS NULL), N'Animaux', 'animaux/animaux', 1, 1, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Animaux' AND ParentCategoryId IS NULL), N'Accessoires animaux', 'animaux/accessoires', 1, 2, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Animaux' AND ParentCategoryId IS NULL), N'Animaux perdus', 'animaux/perdus', 1, 3, 1);
