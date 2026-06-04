-- ============================================
-- Index pour accélérer les recherches d'annonces
-- ============================================

-- Index sur le titre pour la recherche par mots-clés
CREATE NONCLUSTERED INDEX IX_Annonces_Title ON Annonces (Title);

-- Index sur CategoryId pour les jointures
CREATE NONCLUSTERED INDEX IX_Annonces_CategoryId_Title ON Annonces (CategoryId, Title);
