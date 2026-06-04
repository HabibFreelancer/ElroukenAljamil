-- ============================================
-- Configurer ShowInDeposit pour les catégories
-- Par défaut toutes sont à 0 (non visible dans dépôt)
-- On active seulement celles qu'on veut afficher
-- ============================================

-- Activer toutes les catégories racines par défaut
UPDATE Categories SET ShowInDeposit = 1 WHERE ParentCategoryId IS NULL;

-- Exemple : Pour Emploi, n'afficher que "Offres d'emploi"
-- D'abord désactiver toutes les catégories Emploi
UPDATE Categories SET ShowInDeposit = 0 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'emploi');

-- Activer seulement "Offres d'emploi"
UPDATE Categories SET ShowInDeposit = 1 
WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'emploi') 
AND Name = N'Offres d''emploi' AND ParentCategoryId IS NULL;

-- Activer "Formations professionnelles"
--UPDATE Categories SET ShowInDeposit = 1 
--WHERE MenuId = (SELECT Id FROM Menus WHERE Slug = 'emploi') 
--AND Name = N'Formations professionnelles' AND ParentCategoryId IS NULL;
