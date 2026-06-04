-- ============================================
-- Insertion des annonces avec CategoryId
-- La colonne Category a été remplacée par CategoryId (FK vers Categories)
-- ============================================

-- Note: Les CategoryId ci-dessous doivent correspondre aux IDs de votre table Categories
-- Exécutez d'abord : SELECT Id, Name, MenuId FROM Categories WHERE ParentCategoryId IS NULL
-- pour vérifier les IDs disponibles

-- Annonces Véhicules > Voitures
INSERT INTO Annonces (Title, Description, Price, CategoryId, CreatedAt) VALUES
(N'Peugeot 308 2019 - Excellent état', N'Peugeot 308 année 2019, 45000 km, essence, boîte automatique.', 12500.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Voitures' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vehicules')), GETDATE()),
(N'Renault Clio V - Comme neuve', N'Renault Clio V 2021, 20000 km, diesel, GPS intégré.', 9800.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Voitures' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vehicules')), GETDATE()),
(N'BMW Série 3 320d 2020', N'BMW 320d pack M, cuir, toit ouvrant, 60000 km.', 28000.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Voitures' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vehicules')), GETDATE());

-- Annonces Immobilier > Ventes immobilières
INSERT INTO Annonces (Title, Description, Price, CategoryId, CreatedAt) VALUES
(N'Appartement S+2 Lac 2', N'Appartement S+2 au Lac 2, 85m², vue sur le lac, 2ème étage.', 350000.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Ventes immobilières' AND MenuId=(SELECT Id FROM Menus WHERE Slug='immobilier')), GETDATE()),
(N'Villa avec jardin Soukra', N'Villa S+4 avec jardin 200m², piscine, garage double.', 850000.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Ventes immobilières' AND MenuId=(SELECT Id FROM Menus WHERE Slug='immobilier')), GETDATE());

-- Annonces Immobilier > Locations
INSERT INTO Annonces (Title, Description, Price, CategoryId, CreatedAt) VALUES
(N'Studio meublé centre Tunis', N'Studio meublé 35m² au centre-ville, idéal étudiant.', 550.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Locations' AND MenuId=(SELECT Id FROM Menus WHERE Slug='immobilier')), GETDATE());

-- Annonces Électronique > Téléphones
INSERT INTO Annonces (Title, Description, Price, CategoryId, CreatedAt) VALUES
(N'iPhone 14 Pro 128Go', N'iPhone 14 Pro 128Go, noir sidéral, état impeccable, batterie 92%.', 650.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Téléphones & Objets connectés' AND MenuId=(SELECT Id FROM Menus WHERE Slug='electronique')), GETDATE()),
(N'Samsung Galaxy S23 Ultra', N'Samsung Galaxy S23 Ultra 256Go, sous garantie.', 480.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Téléphones & Objets connectés' AND MenuId=(SELECT Id FROM Menus WHERE Slug='electronique')), GETDATE()),
(N'MacBook Pro M2 2023', N'MacBook Pro 14 pouces M2 Pro, 16Go RAM, 512Go SSD.', 1200.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Ordinateurs' AND MenuId=(SELECT Id FROM Menus WHERE Slug='electronique')), GETDATE());

-- Annonces Maison & Jardin > Ameublement
INSERT INTO Annonces (Title, Description, Price, CategoryId, CreatedAt) VALUES
(N'Table basse scandinave en chêne', N'Table basse style scandinave en chêne massif, 120x60cm.', 120.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Ameublement' AND MenuId=(SELECT Id FROM Menus WHERE Slug='maison-jardin')), GETDATE()),
(N'Canapé 3 places velours vert', N'Canapé 3 places en velours vert sapin, pieds dorés.', 580.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Ameublement' AND MenuId=(SELECT Id FROM Menus WHERE Slug='maison-jardin')), GETDATE());

-- Annonces Mode > Vêtements
INSERT INTO Annonces (Title, Description, Price, CategoryId, CreatedAt) VALUES
(N'Robe été fleurie taille M', N'Robe été légère motif fleuri, taille M, jamais portée.', 25.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Vêtements' AND MenuId=(SELECT Id FROM Menus WHERE Slug='mode')), GETDATE()),
(N'Nike Air Max 90 neuves', N'Nike Air Max 90, taille 42, blanches, neuves dans leur boîte.', 85.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Chaussures' AND MenuId=(SELECT Id FROM Menus WHERE Slug='mode')), GETDATE()),
(N'Montre Casio G-Shock', N'Montre Casio G-Shock modèle GA-2100, noire, neuve.', 75.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Montres & Bijoux' AND MenuId=(SELECT Id FROM Menus WHERE Slug='mode')), GETDATE());

-- Annonces Famille > Équipement bébé
INSERT INTO Annonces (Title, Description, Price, CategoryId, CreatedAt) VALUES
(N'Poussette Yoyo Babyzen', N'Poussette Yoyo Babyzen, pliage compact, nacelle incluse.', 280.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Équipement bébé' AND MenuId=(SELECT Id FROM Menus WHERE Slug='famille')), GETDATE());

-- Annonces Loisirs > Vélos
INSERT INTO Annonces (Title, Description, Price, CategoryId, CreatedAt) VALUES
(N'Vélo électrique Decathlon', N'VTT électrique Riverside 500E, batterie 400Wh, 500km.', 900.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Vélos' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), GETDATE()),
(N'Guitare acoustique Yamaha', N'Guitare acoustique Yamaha F310, parfaite pour débutant.', 95.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Instruments de musique' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), GETDATE());

-- Annonces Électronique > Consoles
INSERT INTO Annonces (Title, Description, Price, CategoryId, CreatedAt) VALUES
(N'PlayStation 5 + 2 manettes', N'PS5 version disque, 2 manettes DualSense, 3 jeux inclus.', 350.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Consoles' AND MenuId=(SELECT Id FROM Menus WHERE Slug='electronique')), GETDATE());

-- Annonces Emploi > Offres d'emploi
INSERT INTO Annonces (Title, Description, Price, CategoryId, CreatedAt) VALUES
(N'Développeur Web - CDI Tunis', N'Recherche développeur web Angular/Node.js, 3 ans expérience minimum.', 0.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Offres d''emploi' AND MenuId=(SELECT Id FROM Menus WHERE Slug='emploi')), GETDATE());

-- Annonces Vacances > Types d'hébergements
INSERT INTO Annonces (Title, Description, Price, CategoryId, CreatedAt) VALUES
(N'Location vacances Hammamet vue mer', N'Appartement S+1 vue mer à Hammamet, climatisé, piscine résidence.', 150.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Types d''hébergements' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vacances')), GETDATE());

-- Annonces de test "Jebra" pour la suggestion de catégories
INSERT INTO Annonces (Title, Description, Price, CategoryId, CreatedAt) VALUES
(N'Jebra - Livre ancien collection', N'Livre rare Jebra, édition originale, très bon état.', 45.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Livres' AND MenuId=(SELECT Id FROM Menus WHERE Slug='loisirs')), GETDATE()),
(N'Jebra déco murale artisanale', N'Décoration murale Jebra fait main, style traditionnel tunisien.', 85.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Décoration' AND MenuId=(SELECT Id FROM Menus WHERE Slug='maison-jardin')), GETDATE()),
(N'Jebra - Accessoire moto cuir', N'Sacoche moto en cuir Jebra, résistante, fixation universelle.', 120.00, (SELECT TOP 1 Id FROM Categories WHERE Name=N'Motos' AND MenuId=(SELECT Id FROM Menus WHERE Slug='vehicules')), GETDATE());
