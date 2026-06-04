-- ============================================
-- Supprimer et réinsérer les catégories du menu Maison & Jardin
-- ============================================

-- Supprimer d'abord les sous-catégories (enfants), puis les catégories racines
DELETE FROM Categories WHERE ParentCategoryId IN (SELECT Id FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'maison-jardin'));
DELETE FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'maison-jardin');

-- Insérer les catégories racines
DECLARE @menuId INT = (SELECT Id FROM Menus WHERE Slug = 'maison-jardin');

INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, NULL, N'Ameublement', 'ameublement', 1, 1, 1),
(@menuId, NULL, N'Papeterie & Fournitures scolaires', 'papeterie', 1, 2, 1),
(@menuId, NULL, N'Électroménager', 'electromenager', 1, 3, 1),
(@menuId, NULL, N'Arts de la table', 'arts-table', 1, 4, 1),
(@menuId, NULL, N'Décoration', 'decoration', 1, 5, 1),
(@menuId, NULL, N'Linge de maison', 'linge', 1, 6, 1),
(@menuId, NULL, N'Bricolage', 'bricolage', 1, 7, 1),
(@menuId, NULL, N'Jardin & Plantes', 'jardin-plantes', 1, 8, 1),
(@menuId, NULL, N'Services de jardinerie & bricolage', 'services', 1, 9, 1);

-- Sous-catégories Ameublement
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Ameublement' AND ParentCategoryId IS NULL), N'Armoire', 'ameublement/armoire', 1, 1, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Ameublement' AND ParentCategoryId IS NULL), N'Buffet', 'ameublement/buffet', 1, 2, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Ameublement' AND ParentCategoryId IS NULL), N'Canapé', 'ameublement/canape', 1, 3, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Ameublement' AND ParentCategoryId IS NULL), N'Chaise, tabouret et banc', 'ameublement/chaise', 1, 4, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Ameublement' AND ParentCategoryId IS NULL), N'Fauteuil', 'ameublement/fauteuil', 1, 5, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Ameublement' AND ParentCategoryId IS NULL), N'Lit', 'ameublement/lit', 1, 6, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Ameublement' AND ParentCategoryId IS NULL), N'Meuble de cuisine', 'ameublement/cuisine', 1, 7, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Ameublement' AND ParentCategoryId IS NULL), N'Table de salle à manger', 'ameublement/table', 1, 8, 1);

-- Sous-catégories Électroménager
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Électroménager' AND ParentCategoryId IS NULL), N'Aspirateur', 'electromenager/aspirateur', 1, 1, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Électroménager' AND ParentCategoryId IS NULL), N'Congélateur', 'electromenager/congelateur', 1, 2, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Électroménager' AND ParentCategoryId IS NULL), N'Four', 'electromenager/four', 1, 3, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Électroménager' AND ParentCategoryId IS NULL), N'Lave-linge', 'electromenager/lave-linge', 1, 4, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Électroménager' AND ParentCategoryId IS NULL), N'Lave-vaisselle', 'electromenager/lave-vaisselle', 1, 5, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Électroménager' AND ParentCategoryId IS NULL), N'Micro-ondes', 'electromenager/micro-ondes', 1, 6, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Électroménager' AND ParentCategoryId IS NULL), N'Réfrigérateur', 'electromenager/refrigerateur', 1, 7, 1);

-- Sous-catégories Arts de la table
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Arts de la table' AND ParentCategoryId IS NULL), N'Assiette', 'arts-table/assiette', 1, 1, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Arts de la table' AND ParentCategoryId IS NULL), N'Service de vaisselle', 'arts-table/service-vaisselle', 1, 2, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Arts de la table' AND ParentCategoryId IS NULL), N'Verre', 'arts-table/verre', 1, 3, 1);

-- Sous-catégories Décoration
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Décoration' AND ParentCategoryId IS NULL), N'Applique', 'decoration/applique', 1, 1, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Décoration' AND ParentCategoryId IS NULL), N'Horloge, pendule et réveil', 'decoration/horloge', 1, 2, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Décoration' AND ParentCategoryId IS NULL), N'Lampadaire', 'decoration/lampadaire', 1, 3, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Décoration' AND ParentCategoryId IS NULL), N'Lampe à poser', 'decoration/lampe', 1, 4, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Décoration' AND ParentCategoryId IS NULL), N'Lustre', 'decoration/lustre', 1, 5, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Décoration' AND ParentCategoryId IS NULL), N'Miroir', 'decoration/miroir', 1, 6, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Décoration' AND ParentCategoryId IS NULL), N'Rideaux, voilage et store', 'decoration/rideaux', 1, 7, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Décoration' AND ParentCategoryId IS NULL), N'Sculpture et statue', 'decoration/sculpture', 1, 8, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Décoration' AND ParentCategoryId IS NULL), N'Suspension', 'decoration/suspension', 1, 9, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Décoration' AND ParentCategoryId IS NULL), N'Tableau et toile', 'decoration/tableau', 1, 10, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Décoration' AND ParentCategoryId IS NULL), N'Tapis', 'decoration/tapis', 1, 11, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Décoration' AND ParentCategoryId IS NULL), N'Vase, cache pot et céramique', 'decoration/vase', 1, 12, 1);

-- Sous-catégories Linge de maison
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Linge de maison' AND ParentCategoryId IS NULL), N'Équipement du lit', 'linge/equipement-lit', 1, 1, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Linge de maison' AND ParentCategoryId IS NULL), N'Déco textile', 'linge/deco-textile', 1, 2, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Linge de maison' AND ParentCategoryId IS NULL), N'Linge de bain', 'linge/bain', 1, 3, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Linge de maison' AND ParentCategoryId IS NULL), N'Linge de lit', 'linge/lit', 1, 4, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Linge de maison' AND ParentCategoryId IS NULL), N'Linge de table', 'linge/table', 1, 5, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Linge de maison' AND ParentCategoryId IS NULL), N'Autre', 'linge/autre', 1, 6, 1);
