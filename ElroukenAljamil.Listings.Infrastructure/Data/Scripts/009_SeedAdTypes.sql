-- ============================================
-- Insertion des AdTypes (types de demande par catégorie)
-- Supprimer les anciennes données et réinsérer
-- ============================================

DELETE FROM AdTypes;

-- ============================================
-- EMPLOI
-- ============================================
INSERT INTO AdTypes (CategoryId, Label, Description, IsDefault, DisplayOrder, IsActive) VALUES
((SELECT Id FROM Categories WHERE Name=N'Offres d''emploi' AND MenuId=(SELECT Id FROM Menus WHERE Slug='emploi')), N'Demande', N'Vous recherchez un emploi.', 1, 1, 1);

-- ============================================
-- VÉHICULES
-- ============================================
INSERT INTO AdTypes (CategoryId, Label, Description, IsDefault, DisplayOrder, IsActive) VALUES
((SELECT Id FROM Categories WHERE Name=N'Voitures' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vehicules')), N'Offre', N'Vous vendez un véhicule.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Voitures' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vehicules')), N'Demande', N'Vous recherchez un véhicule.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Motos' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vehicules')), N'Offre', N'Vous vendez une moto.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Motos' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vehicules')), N'Demande', N'Vous recherchez une moto.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Caravaning' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vehicules')), N'Offre', N'Vous vendez un véhicule de loisirs.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Caravaning' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vehicules')), N'Demande', N'Vous recherchez une caravane.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Utilitaires' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vehicules')), N'Offre', N'Vous vendez un utilitaire.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Utilitaires' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vehicules')), N'Demande', N'Vous recherchez un utilitaire.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Nautisme' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vehicules')), N'Offre', N'Vous vendez un bateau.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Nautisme' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vehicules')), N'Demande', N'Vous recherchez un bateau.', 0, 2, 1);

-- ============================================
-- IMMOBILIER > Bureaux & Commerces
-- ============================================
INSERT INTO AdTypes (CategoryId, Label, Description, IsDefault, DisplayOrder, IsActive) VALUES
((SELECT Id FROM Categories WHERE Name=N'Bureau & Commerce' AND MenuId=(SELECT Id FROM Menus WHERE Slug='immobilier')), N'Vente', N'Vous vendez un bien immobilier.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Bureau & Commerce' AND MenuId=(SELECT Id FROM Menus WHERE Slug='immobilier')), N'Location', N'Vous proposez un bien en location.', 0, 2, 1);

-- ============================================
-- VACANCES
-- ============================================
INSERT INTO AdTypes (CategoryId, Label, Description, IsDefault, DisplayOrder, IsActive) VALUES
((SELECT Id FROM Categories WHERE Name=N'Types d''hébergements' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vacances')), N'Offre', N'Vous proposez une location de vacances ou un gîte.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Types d''hébergements' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vacances')), N'Demande', N'Vous recherchez une location de vacances ou un gîte.', 0, 2, 1);

-- ============================================
-- ÉLECTRONIQUE
-- ============================================
INSERT INTO AdTypes (CategoryId, Label, Description, IsDefault, DisplayOrder, IsActive) VALUES
((SELECT Id FROM Categories WHERE Name=N'Ordinateurs' AND MenuId=(SELECT Id FROM Menus WHERE Slug='electronique')), N'Offre', N'Vous vendez un bien informatique.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Accessoires informatique' AND MenuId=(SELECT Id FROM Menus WHERE Slug='electronique')), N'Offre', N'Vous proposez des accessoires pour ordinateur.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Tablettes & Liseuses' AND MenuId=(SELECT Id FROM Menus WHERE Slug='electronique')), N'Offre', N'Vous proposez une tablette ou une liseuse.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Photo, audio & vidéo' AND MenuId=(SELECT Id FROM Menus WHERE Slug='electronique')), N'Offre', N'Vous vendez un bien.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Téléphones & Objets connectés' AND MenuId=(SELECT Id FROM Menus WHERE Slug='electronique')), N'Offre', N'Vous vendez un bien.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Accessoires téléphone & Objets connectés' AND MenuId=(SELECT Id FROM Menus WHERE Slug='electronique')), N'Offre', N'Vous proposez des accessoires pour téléphone.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Consoles' AND MenuId=(SELECT Id FROM Menus WHERE Slug='electronique')), N'Offre', N'Vous vendez une console ou un jeu vidéo.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Jeux vidéo' AND MenuId=(SELECT Id FROM Menus WHERE Slug='electronique')), N'Offre', N'Vous proposez des jeux vidéos.', 1, 1, 1);

-- ============================================
-- MAISON & JARDIN
-- ============================================
INSERT INTO AdTypes (CategoryId, Label, Description, IsDefault, DisplayOrder, IsActive) VALUES
((SELECT Id FROM Categories WHERE Name=N'Ameublement' AND MenuId=(SELECT Id FROM Menus WHERE Slug='maison-jardin')), N'Offre', N'Vous proposez un meuble.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Électroménager' AND MenuId=(SELECT Id FROM Menus WHERE Slug='maison-jardin')), N'Offre', N'Vous vendez de l''électroménager.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Arts de la table' AND MenuId=(SELECT Id FROM Menus WHERE Slug='maison-jardin')), N'Offre', N'Vous vendez un objet d''art de la table.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Décoration' AND MenuId=(SELECT Id FROM Menus WHERE Slug='maison-jardin')), N'Offre', N'Vous proposez un objet de décoration.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Linge de maison' AND MenuId=(SELECT Id FROM Menus WHERE Slug='maison-jardin')), N'Offre', N'Vous vendez du linge de maison.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Bricolage' AND MenuId=(SELECT Id FROM Menus WHERE Slug='maison-jardin')), N'Offre', N'Vous proposez un bien.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Jardin & Plantes' AND MenuId=(SELECT Id FROM Menus WHERE Slug='maison-jardin')), N'Offre', N'Vous proposez un bien.', 1, 1, 1);

-- ============================================
-- FAMILLE
-- ============================================
INSERT INTO AdTypes (CategoryId, Label, Description, IsDefault, DisplayOrder, IsActive) VALUES
((SELECT Id FROM Categories WHERE Name=N'Équipement bébé' AND MenuId=(SELECT Id FROM Menus WHERE Slug='famille')), N'Offre', N'Vous vendez des objets pour bébé.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Mobilier enfant' AND MenuId=(SELECT Id FROM Menus WHERE Slug='famille')), N'Offre', N'Vous proposez un meuble pour chambre d''enfant.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Vêtements bébé' AND MenuId=(SELECT Id FROM Menus WHERE Slug='famille')), N'Offre', N'Vous vendez un vêtement pour bébé.', 1, 1, 1);

-- ============================================
-- MODE
-- ============================================
INSERT INTO AdTypes (CategoryId, Label, Description, IsDefault, DisplayOrder, IsActive) VALUES
((SELECT Id FROM Categories WHERE Name=N'Vêtements' AND MenuId=(SELECT Id FROM Menus WHERE Slug='mode')), N'Offre', N'Vous vendez un vêtement.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Chaussures' AND MenuId=(SELECT Id FROM Menus WHERE Slug='mode')), N'Offre', N'Vous vendez des chaussures.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Accessoires & Bagagerie' AND MenuId=(SELECT Id FROM Menus WHERE Slug='mode')), N'Offre', N'Vous proposez un accessoire ou un bagage.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Montres & Bijoux' AND MenuId=(SELECT Id FROM Menus WHERE Slug='mode')), N'Offre', N'Vous proposez un bien.', 1, 1, 1);

-- ============================================
-- LOISIRS
-- ============================================
INSERT INTO AdTypes (CategoryId, Label, Description, IsDefault, DisplayOrder, IsActive) VALUES
((SELECT Id FROM Categories WHERE Name=N'Antiquités' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), N'Offre', N'Vous offrez une antiquité.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Collection' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), N'Offre', N'Vous vendez un bien.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'CD - Musique' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), N'Offre', N'Vous proposez un bien.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'DVD - Films' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), N'Offre', N'Vous proposez un bien.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Instruments de musique' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), N'Offre', N'Vous vendez un instrument.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Livres' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), N'Offre', N'Vous vendez un livre.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Modélisme' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), N'Offre', N'Vous proposez une maquette.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Vins & Gastronomie' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), N'Offre', N'Vous vendez un bien.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Jeux & Jouets' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), N'Offre', N'Vous vendez un bien.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Loisirs créatifs' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), N'Offre', N'Vous proposez des produits de couture.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Sport & Plein air' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), N'Offre', N'Vous vendez un bien.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Vélos' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), N'Offre', N'Vous vendez un vélo.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Équipements vélos' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), N'Offre', N'Vous proposez des équipements pour vélo.', 1, 1, 1);

-- ============================================
-- AUTRES > Animaux (sous-catégories)
-- ============================================
INSERT INTO AdTypes (CategoryId, Label, Description, IsDefault, DisplayOrder, IsActive) VALUES
((SELECT TOP 1 Id FROM Categories WHERE Name=N'Animaux' AND ParentCategoryId IS NOT NULL AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous vendez un animal.', 1, 1, 1),
((SELECT TOP 1 Id FROM Categories WHERE Name=N'Animaux' AND ParentCategoryId IS NOT NULL AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous cherchez un animal.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Accessoires animaux' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un accessoire pour animaux.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Animaux perdus' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous avez trouvé un animal.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Animaux perdus' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous avez perdu un animal.', 0, 2, 1);

-- ============================================
-- AUTRES > Matériel professionnel (sous-catégories)
-- ============================================
INSERT INTO AdTypes (CategoryId, Label, Description, IsDefault, DisplayOrder, IsActive) VALUES
((SELECT Id FROM Categories WHERE Name=N'Tracteurs' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un tracteur.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Matériel agricole' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous vendez du matériel agricole.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'BTP - Chantier gros-oeuvre' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous vendez du matériel BTP.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Poids lourds' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un camion poids lourd.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Manutention - Levage' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous vendez du matériel de transport ou de manutention.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Équipements industriels' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous vendez des équipements industriels.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Équipements pour restaurants & hôtels' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous vendez du matériel de restauration - hôtellerie.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Équipements & Fournitures de bureau' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous vendez des fournitures de bureau.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Équipements pour commerces & marchés' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous vendez du matériel de commerces & marchés.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Matériel médical' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous vendez du matériel médical.', 1, 1, 1);

-- ============================================
-- AUTRES > Services (sous-catégories)
-- ============================================
INSERT INTO AdTypes (CategoryId, Label, Description, IsDefault, DisplayOrder, IsActive) VALUES
((SELECT Id FROM Categories WHERE Name=N'Artistes & Musiciens' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un service.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Artistes & Musiciens' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous cherchez un service.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Baby-Sitting' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un service.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Baby-Sitting' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous cherchez un service.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Billetterie' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un billet.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Billetterie' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous recherchez un billet.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Covoiturage' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un covoiturage.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Covoiturage' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous recherchez un covoiturage.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Cours particuliers' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez de donner des cours particuliers.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Cours particuliers' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous recherchez un professeur de cours particuliers.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Entraide entre voisins' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez votre aide.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Entraide entre voisins' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous cherchez de l''aide.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Évènements' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un évènement.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Évènements' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous recherchez un évènement.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Services à la personne' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un service.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Services à la personne' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous cherchez un service.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Services aux animaux' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un service.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Services aux animaux' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous cherchez un service.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Services de déménagement' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un service.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Services de déménagement' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous cherchez un service.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Services de réparations électroniques' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un service.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Services de réparations électroniques' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous cherchez un service.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Services de jardinerie & bricolage' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un service.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Services de jardinerie & bricolage' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous cherchez un service.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Services évènementiels' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un service.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Services évènementiels' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous cherchez un service.', 0, 2, 1),
((SELECT Id FROM Categories WHERE Name=N'Autres services' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez vos services.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Autres services' AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous recherchez une aide.', 0, 2, 1);

-- ============================================
-- AUTRES > Divers
-- ============================================
INSERT INTO AdTypes (CategoryId, Label, Description, IsDefault, DisplayOrder, IsActive) VALUES
((SELECT Id FROM Categories WHERE Name=N'Autres' AND ParentCategoryId IS NULL AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Offre', N'Vous proposez un bien.', 1, 1, 1),
((SELECT Id FROM Categories WHERE Name=N'Autres' AND ParentCategoryId IS NULL AND MenuId=(SELECT Id FROM Menus WHERE Slug='autre')), N'Demande', N'Vous recherchez un bien.', 0, 2, 1);
