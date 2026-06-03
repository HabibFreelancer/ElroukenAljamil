-- ============================================
-- Supprimer et réinsérer les catégories du menu Famille
-- ============================================

-- Supprimer d'abord les sous-catégories (enfants), puis les catégories racines
DELETE FROM Categories WHERE ParentCategoryId IN (SELECT Id FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'famille'));
DELETE FROM Categories WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'famille');

-- Insérer les catégories racines
DECLARE @menuId INT = (SELECT Id FROM Menus WHERE Slug = 'famille');

INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, NULL, N'Équipement bébé', 'equipement-bebe', 1, 1, 1),
(@menuId, NULL, N'Mobilier enfant', 'mobilier-enfant', 1, 2, 1),
(@menuId, NULL, N'Vêtements bébé', 'vetements-bebe', 1, 3, 1),
(@menuId, NULL, N'Vêtements enfants', 'vetements-enfants', 1, 4, 1),
(@menuId, NULL, N'Vêtements maternité', 'vetements-maternite', 1, 5, 1),
(@menuId, NULL, N'Chaussures enfants', 'chaussures-enfants', 1, 6, 1),
(@menuId, NULL, N'Montres & bijoux enfants', 'montres-bijoux-enfants', 1, 7, 1),
(@menuId, NULL, N'Accessoires & bagagerie enfants', 'accessoires-enfants', 1, 8, 1),
(@menuId, NULL, N'Jeux & Jouets', 'jeux-jouets', 1, 9, 1),
(@menuId, NULL, N'Baby-Sitting', 'baby-sitting', 1, 10, 1);

-- Sous-catégories Équipement bébé
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Équipement bébé' AND ParentCategoryId IS NULL), N'Poussette', 'equipement-bebe/poussette', 1, 1, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Équipement bébé' AND ParentCategoryId IS NULL), N'Siège auto', 'equipement-bebe/siege-auto', 1, 2, 1);

-- Sous-catégories Mobilier enfant
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Mobilier enfant' AND ParentCategoryId IS NULL), N'Baignoire', 'mobilier-enfant/baignoire', 1, 1, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Mobilier enfant' AND ParentCategoryId IS NULL), N'Chaise haute', 'mobilier-enfant/chaise-haute', 1, 2, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Mobilier enfant' AND ParentCategoryId IS NULL), N'Lit bébé', 'mobilier-enfant/lit-bebe', 1, 3, 1);

-- Sous-catégories Vêtements bébé
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Vêtements bébé' AND ParentCategoryId IS NULL), N'0 mois à 3 mois', 'vetements-bebe/0-3', 1, 1, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Vêtements bébé' AND ParentCategoryId IS NULL), N'3 mois à 6 mois', 'vetements-bebe/3-6', 1, 2, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Vêtements bébé' AND ParentCategoryId IS NULL), N'6 mois à 9 mois', 'vetements-bebe/6-9', 1, 3, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Vêtements bébé' AND ParentCategoryId IS NULL), N'9 mois à 12 mois', 'vetements-bebe/9-12', 1, 4, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Vêtements bébé' AND ParentCategoryId IS NULL), N'12 mois à 18 mois', 'vetements-bebe/12-18', 1, 5, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Vêtements bébé' AND ParentCategoryId IS NULL), N'18 mois à 24 mois', 'vetements-bebe/18-24', 1, 6, 1),
(@menuId, (SELECT Id FROM Categories WHERE MenuId=@menuId AND Name=N'Vêtements bébé' AND ParentCategoryId IS NULL), N'Plus de 24 mois', 'vetements-bebe/24-plus', 1, 7, 1);
