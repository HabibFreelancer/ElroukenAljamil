-- ============================================
-- Supprimer et réinsérer les catégories du menu Loisirs
-- ============================================

-- Supprimer d'abord les sous-catégories (enfants), puis les catégories racines
DELETE FROM Categories WHERE ParentCategoryId IN (SELECT Id FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'loisirs'));
DELETE FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'loisirs');

-- Insérer les catégories racines
DECLARE @menuId INT = (SELECT Id FROM Menus WHERE Slug = 'loisirs');

INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, NULL, N'Antiquités', 'antiquites', 1, 1, 1),
(@menuId, NULL, N'Artistes & Musiciens', 'artistes-musiciens', 1, 2, 1),
(@menuId, NULL, N'Billetterie', 'billetterie', 1, 3, 1),
(@menuId, NULL, N'Collection', 'collection', 1, 4, 1),
(@menuId, NULL, N'CD - Musique', 'cd-musique', 1, 5, 1),
(@menuId, NULL, N'DVD - Films', 'dvd-films', 1, 6, 1),
(@menuId, NULL, N'Instruments de musique', 'instruments-musique', 1, 7, 1),
(@menuId, NULL, N'Livres', 'livres', 1, 8, 1),
(@menuId, NULL, N'Modélisme', 'modelisme', 1, 9, 1),
(@menuId, NULL, N'Vins & Gastronomie', 'vins-gastronomie', 1, 10, 1),
(@menuId, NULL, N'Jeux & Jouets', 'jeux-jouets', 1, 11, 1),
(@menuId, NULL, N'Loisirs créatifs', 'loisirs-creatifs', 1, 12, 1),
(@menuId, NULL, N'Sport & Plein air', 'sport-plein-air', 1, 13, 1),
(@menuId, NULL, N'Vélos', 'velos', 1, 14, 1),
(@menuId, NULL, N'Équipements vélos', 'equipements-velos', 1, 15, 1);

-- Sous-catégories Jeux & Jouets
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Jeux & Jouets' AND ParentCategoryId IS NULL), N'Jeux de société', 'jeux-jouets/jeux-societe', 1, 1, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Jeux & Jouets' AND ParentCategoryId IS NULL), N'Poupées et accessoires', 'jeux-jouets/poupees', 1, 2, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Jeux & Jouets' AND ParentCategoryId IS NULL), N'Porteurs, trotteurs et draisiennes', 'jeux-jouets/porteurs', 1, 3, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Jeux & Jouets' AND ParentCategoryId IS NULL), N'Jouets d''éveil', 'jeux-jouets/eveil', 1, 4, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Jeux & Jouets' AND ParentCategoryId IS NULL), N'Cuisines et dînettes', 'jeux-jouets/cuisines', 1, 5, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Jeux & Jouets' AND ParentCategoryId IS NULL), N'Jeux de construction', 'jeux-jouets/construction', 1, 6, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Jeux & Jouets' AND ParentCategoryId IS NULL), N'Voitures et circuits', 'jeux-jouets/voitures-circuits', 1, 7, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Jeux & Jouets' AND ParentCategoryId IS NULL), N'Puzzle', 'jeux-jouets/puzzle', 1, 8, 1);

-- Sous-catégories Vélos
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Vélos' AND ParentCategoryId IS NULL), N'Vélo de route', 'velos/route', 1, 1, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Vélos' AND ParentCategoryId IS NULL), N'VTT', 'velos/vtt', 1, 2, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Vélos' AND ParentCategoryId IS NULL), N'Vélo électrique', 'velos/electrique', 1, 3, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Vélos' AND ParentCategoryId IS NULL), N'Vélo enfant', 'velos/enfant', 1, 4, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Vélos' AND ParentCategoryId IS NULL), N'VTC', 'velos/vtc', 1, 5, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Vélos' AND ParentCategoryId IS NULL), N'Vélo de ville', 'velos/ville', 1, 6, 1);
