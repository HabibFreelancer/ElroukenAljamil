-- ============================================
-- ElroukenAljamil - Script SQL
-- Insertion des données : Menus, Categories, Annonces
-- ============================================

-- ============================================
-- INSERTION DES MENUS
-- ============================================
INSERT INTO Menus (Name, Slug, Icon, DisplayOrder, IsActive) VALUES
(N'Immobilier', 'immobilier', 'fa-solid fa-house', 1, 1),
(N'Véhicules', 'vehicules', 'fa-solid fa-car', 2, 1),
(N'Vacances', 'vacances', 'fa-solid fa-umbrella-beach', 3, 1),
(N'Emploi', 'emploi', 'fa-solid fa-briefcase', 4, 1),
(N'Mode', 'mode', 'fa-solid fa-shirt', 5, 1),
(N'Maison & Jardin', 'maison-jardin', 'fa-solid fa-couch', 6, 1),
(N'Famille', 'famille', 'fa-solid fa-baby', 7, 1),
(N'Électronique', 'electronique', 'fa-solid fa-mobile-screen', 8, 1),
(N'Loisirs', 'loisirs', 'fa-solid fa-futbol', 9, 1),
(N'Autres', 'autre', 'fa-solid fa-ellipsis', 10, 1);

-- ============================================
-- IMMOBILIER (MenuId = 1)
-- ============================================
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(1, NULL, N'Ventes immobilières', 'ventes', 1, 1, 1),
(1, NULL, N'Immobilier Neuf', 'neuf', 1, 2, 1),
(1, NULL, N'Locations', 'location', 1, 3, 1),
(1, NULL, N'Colocations', 'colocation', 1, 4, 1),
(1, NULL, N'Bureau & Commerce', 'bureau-commerce', 1, 5, 1),
(1, NULL, N'Service de déménagement', 'demenagement', 1, 6, 1);

-- Sous-catégories Ventes immobilières
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(1, (SELECT Id FROM Categories WHERE MenuId=1 AND Name=N'Ventes immobilières'), N'Appartement', 'appartement', 1, 1, 1),
(1, (SELECT Id FROM Categories WHERE MenuId=1 AND Name=N'Ventes immobilières'), N'Maison', 'maison', 1, 2, 1),
(1, (SELECT Id FROM Categories WHERE MenuId=1 AND Name=N'Ventes immobilières'), N'Terrain', 'terrain', 1, 3, 1);

-- Sous-catégories Immobilier Neuf
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(1, (SELECT Id FROM Categories WHERE MenuId=1 AND Name=N'Immobilier Neuf'), N'Appartement', 'neuf/appartement', 1, 1, 1),
(1, (SELECT Id FROM Categories WHERE MenuId=1 AND Name=N'Immobilier Neuf'), N'Maison', 'neuf/maison', 1, 2, 1),
(1, (SELECT Id FROM Categories WHERE MenuId=1 AND Name=N'Immobilier Neuf'), N'Programme logement neufs', 'neuf/programmes', 1, 3, 1),
(1, (SELECT Id FROM Categories WHERE MenuId=1 AND Name=N'Immobilier Neuf'), N'Promoteurs immobiliers', 'neuf/promoteurs', 1, 4, 1);

-- Sous-catégories Locations
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(1, (SELECT Id FROM Categories WHERE MenuId=1 AND Name=N'Locations'), N'Appartement', 'location/appartement', 1, 1, 1),
(1, (SELECT Id FROM Categories WHERE MenuId=1 AND Name=N'Locations'), N'Maison', 'location/maison', 1, 2, 1),
(1, (SELECT Id FROM Categories WHERE MenuId=1 AND Name=N'Locations'), N'Parking', 'location/parking', 1, 3, 1);

-- ============================================
-- VÉHICULES (MenuId = 2)
-- ============================================
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(2, NULL, N'Voitures', 'voitures', 1, 1, 1),
(2, NULL, N'Motos', 'motos', 1, 2, 1),
(2, NULL, N'Caravaning', 'caravaning', 1, 3, 1),
(2, NULL, N'Utilitaires', 'utilitaires', 1, 4, 1),
(2, NULL, N'Camions', 'camions', 1, 5, 1),
(2, NULL, N'Nautisme', 'nautisme', 1, 6, 1),
(2, NULL, N'Vélos', 'velos', 1, 7, 1),
(2, NULL, N'Équipement auto', 'equipement-auto', 1, 8, 1),
(2, NULL, N'Équipement moto', 'equipement-moto', 1, 9, 1),
(2, NULL, N'Équipement caravaning', 'equipement-caravaning', 1, 10, 1),
(2, NULL, N'Équipement nautisme', 'equipement-nautisme', 1, 11, 1),
(2, NULL, N'Équipements vélos', 'equipements-velos', 1, 12, 1),
(2, NULL, N'Services de réparations mécaniques', 'reparations', 1, 13, 1);

-- Sous-catégories Voitures
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(2, (SELECT Id FROM Categories WHERE MenuId=2 AND Name=N'Voitures'), N'Audi', 'voitures/audi', 1, 1, 1),
(2, (SELECT Id FROM Categories WHERE MenuId=2 AND Name=N'Voitures'), N'BMW', 'voitures/bmw', 1, 2, 1),
(2, (SELECT Id FROM Categories WHERE MenuId=2 AND Name=N'Voitures'), N'Mercedes', 'voitures/mercedes', 1, 3, 1),
(2, (SELECT Id FROM Categories WHERE MenuId=2 AND Name=N'Voitures'), N'Peugeot', 'voitures/peugeot', 1, 4, 1),
(2, (SELECT Id FROM Categories WHERE MenuId=2 AND Name=N'Voitures'), N'Renault', 'voitures/renault', 1, 5, 1),
(2, (SELECT Id FROM Categories WHERE MenuId=2 AND Name=N'Voitures'), N'Volkswagen', 'voitures/volkswagen', 1, 6, 1);

-- Sous-catégories Motos
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(2, (SELECT Id FROM Categories WHERE MenuId=2 AND Name=N'Motos'), N'BMW', 'motos/bmw', 1, 1, 1),
(2, (SELECT Id FROM Categories WHERE MenuId=2 AND Name=N'Motos'), N'Honda', 'motos/honda', 1, 2, 1),
(2, (SELECT Id FROM Categories WHERE MenuId=2 AND Name=N'Motos'), N'Kawasaki', 'motos/kawasaki', 1, 3, 1),
(2, (SELECT Id FROM Categories WHERE MenuId=2 AND Name=N'Motos'), N'Suzuki', 'motos/suzuki', 1, 4, 1),
(2, (SELECT Id FROM Categories WHERE MenuId=2 AND Name=N'Motos'), N'Yamaha', 'motos/yamaha', 1, 5, 1);

-- ============================================
-- VACANCES (MenuId = 3)
-- ============================================
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(3, NULL, N'Types d''hébergements', 'types', 1, 1, 1),
(3, NULL, N'Caractéristiques recherchées', 'caracteristiques', 0, 2, 1),
(3, NULL, N'Nombre de voyageurs', 'voyageurs', 0, 3, 1);

INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(3, (SELECT Id FROM Categories WHERE MenuId=3 AND Name=N'Types d''hébergements'), N'Maisons et villas', 'maisons-villas', 1, 1, 1),
(3, (SELECT Id FROM Categories WHERE MenuId=3 AND Name=N'Types d''hébergements'), N'Appartements', 'appartements', 1, 2, 1),
(3, (SELECT Id FROM Categories WHERE MenuId=3 AND Name=N'Types d''hébergements'), N'Chalets', 'chalets', 1, 3, 1),
(3, (SELECT Id FROM Categories WHERE MenuId=3 AND Name=N'Types d''hébergements'), N'Chambres d''hôtes', 'chambres-hotes', 1, 4, 1),
(3, (SELECT Id FROM Categories WHERE MenuId=3 AND Name=N'Types d''hébergements'), N'Campings', 'campings', 1, 5, 1);

INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(3, (SELECT Id FROM Categories WHERE MenuId=3 AND Name=N'Caractéristiques recherchées'), N'Piscine', 'piscine', 1, 1, 1),
(3, (SELECT Id FROM Categories WHERE MenuId=3 AND Name=N'Caractéristiques recherchées'), N'Jardin', 'jardin', 1, 2, 1),
(3, (SELECT Id FROM Categories WHERE MenuId=3 AND Name=N'Caractéristiques recherchées'), N'Animaux acceptés', 'animaux', 1, 3, 1);

INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(3, (SELECT Id FROM Categories WHERE MenuId=3 AND Name=N'Nombre de voyageurs'), N'Solo', 'solo', 1, 1, 1),
(3, (SELECT Id FROM Categories WHERE MenuId=3 AND Name=N'Nombre de voyageurs'), N'À deux', 'a-deux', 1, 2, 1),
(3, (SELECT Id FROM Categories WHERE MenuId=3 AND Name=N'Nombre de voyageurs'), N'À quatre', 'a-quatre', 1, 3, 1),
(3, (SELECT Id FROM Categories WHERE MenuId=3 AND Name=N'Nombre de voyageurs'), N'À six', 'a-six', 1, 4, 1),
(3, (SELECT Id FROM Categories WHERE MenuId=3 AND Name=N'Nombre de voyageurs'), N'Plus de six', 'plus-de-six', 1, 5, 1);

-- ============================================
-- EMPLOI (MenuId = 4)
-- ============================================
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(4, NULL, N'Offres d''emploi', 'offres', 1, 1, 1),
(4, NULL, N'Formations professionnelles', 'formations', 1, 2, 1),
(4, NULL, N'Profil Candidat', 'profil', 1, 3, 1);

INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(4, (SELECT Id FROM Categories WHERE MenuId=4 AND Name=N'Offres d''emploi'), N'Intérim', 'interim', 1, 1, 1),
(4, (SELECT Id FROM Categories WHERE MenuId=4 AND Name=N'Offres d''emploi'), N'CDI', 'cdi', 1, 2, 1),
(4, (SELECT Id FROM Categories WHERE MenuId=4 AND Name=N'Offres d''emploi'), N'CDD', 'cdd', 1, 3, 1),
(4, (SELECT Id FROM Categories WHERE MenuId=4 AND Name=N'Offres d''emploi'), N'Bénévolat', 'benevolat', 1, 4, 1),
(4, (SELECT Id FROM Categories WHERE MenuId=4 AND Name=N'Offres d''emploi'), N'Autre', 'autre', 1, 5, 1);

-- ============================================
-- MODE (MenuId = 5)
-- ============================================
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(5, NULL, N'Vêtements', 'vetements', 1, 1, 1),
(5, NULL, N'Chaussures', 'chaussures', 1, 2, 1),
(5, NULL, N'Montres & Bijoux', 'montres-bijoux', 1, 3, 1),
(5, NULL, N'Accessoires & Bagagerie', 'accessoires', 1, 4, 1);

INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Vêtements'), N'Femme', 'vetements/femme', 1, 1, 1),
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Vêtements'), N'Maternité', 'vetements/maternite', 1, 2, 1),
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Vêtements'), N'Homme', 'vetements/homme', 1, 3, 1),
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Vêtements'), N'Enfant', 'vetements/enfant', 1, 4, 1);

INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Chaussures'), N'Femme', 'chaussures/femme', 1, 1, 1),
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Chaussures'), N'Homme', 'chaussures/homme', 1, 2, 1),
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Chaussures'), N'Enfant', 'chaussures/enfant', 1, 3, 1);

INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Montres & Bijoux'), N'Femme', 'montres-bijoux/femme', 1, 1, 1),
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Montres & Bijoux'), N'Homme', 'montres-bijoux/homme', 1, 2, 1),
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Montres & Bijoux'), N'Enfant', 'montres-bijoux/enfant', 1, 3, 1),
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Montres & Bijoux'), N'Mixte', 'montres-bijoux/mixte', 1, 4, 1);

INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Accessoires & Bagagerie'), N'Femme', 'accessoires/femme', 1, 1, 1),
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Accessoires & Bagagerie'), N'Homme', 'accessoires/homme', 1, 2, 1),
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Accessoires & Bagagerie'), N'Enfant', 'accessoires/enfant', 1, 3, 1),
(5, (SELECT Id FROM Categories WHERE MenuId=5 AND Name=N'Accessoires & Bagagerie'), N'Mixte', 'accessoires/mixte', 1, 4, 1);

-- ============================================
-- MAISON & JARDIN (MenuId = 6)
-- ============================================
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(6, NULL, N'Ameublement', 'ameublement', 1, 1, 1),
(6, NULL, N'Papeterie & Fournitures scolaires', 'papeterie', 1, 2, 1),
(6, NULL, N'Électroménager', 'electromenager', 1, 3, 1),
(6, NULL, N'Arts de la table', 'arts-table', 1, 4, 1),
(6, NULL, N'Décoration', 'decoration', 1, 5, 1),
(6, NULL, N'Linge de maison', 'linge', 1, 6, 1),
(6, NULL, N'Bricolage', 'bricolage', 1, 7, 1),
(6, NULL, N'Jardin & Plantes', 'jardin-plantes', 1, 8, 1),
(6, NULL, N'Services de jardinerie & bricolage', 'services', 1, 9, 1);

-- ============================================
-- FAMILLE (MenuId = 7)
-- ============================================
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(7, NULL, N'Équipement bébé', 'equipement-bebe', 1, 1, 1),
(7, NULL, N'Mobilier enfant', 'mobilier-enfant', 1, 2, 1),
(7, NULL, N'Vêtements bébé', 'vetements-bebe', 1, 3, 1),
(7, NULL, N'Vêtements enfants', 'vetements-enfants', 1, 4, 1),
(7, NULL, N'Vêtements maternité', 'vetements-maternite', 1, 5, 1),
(7, NULL, N'Chaussures enfants', 'chaussures-enfants', 1, 6, 1),
(7, NULL, N'Montres & bijoux enfants', 'montres-bijoux-enfants', 1, 7, 1),
(7, NULL, N'Accessoires & bagagerie enfants', 'accessoires-enfants', 1, 8, 1),
(7, NULL, N'Jeux & Jouets', 'jeux-jouets', 1, 9, 1),
(7, NULL, N'Baby-Sitting', 'baby-sitting', 1, 10, 1);

-- ============================================
-- ÉLECTRONIQUE (MenuId = 8)
-- ============================================
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(8, NULL, N'Ordinateurs', 'ordinateurs', 1, 1, 1),
(8, NULL, N'Accessoires informatique', 'accessoires-informatique', 1, 2, 1),
(8, NULL, N'Tablettes & Liseuses', 'tablettes-liseuses', 1, 3, 1),
(8, NULL, N'Photo, audio & vidéo', 'photo-audio-video', 1, 4, 1),
(8, NULL, N'Téléphones & Objets connectés', 'telephones', 1, 5, 1),
(8, NULL, N'Accessoires téléphone & Objets connectés', 'accessoires-telephone', 1, 6, 1),
(8, NULL, N'Consoles', 'consoles', 1, 7, 1),
(8, NULL, N'Jeux vidéo', 'jeux-video', 1, 8, 1),
(8, NULL, N'Électroménager', 'electromenager', 1, 9, 1),
(8, NULL, N'Services de réparations électroniques', 'reparations', 1, 10, 1);

INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(8, (SELECT Id FROM Categories WHERE MenuId=8 AND Name=N'Téléphones & Objets connectés'), N'Apple', 'telephones/apple', 1, 1, 1),
(8, (SELECT Id FROM Categories WHERE MenuId=8 AND Name=N'Téléphones & Objets connectés'), N'Samsung', 'telephones/samsung', 1, 2, 1),
(8, (SELECT Id FROM Categories WHERE MenuId=8 AND Name=N'Téléphones & Objets connectés'), N'Huawei', 'telephones/huawei', 1, 3, 1),
(8, (SELECT Id FROM Categories WHERE MenuId=8 AND Name=N'Téléphones & Objets connectés'), N'Sony', 'telephones/sony', 1, 4, 1),
(8, (SELECT Id FROM Categories WHERE MenuId=8 AND Name=N'Téléphones & Objets connectés'), N'One plus', 'telephones/oneplus', 1, 5, 1),
(8, (SELECT Id FROM Categories WHERE MenuId=8 AND Name=N'Téléphones & Objets connectés'), N'Google', 'telephones/google', 1, 6, 1);

-- ============================================
-- LOISIRS (MenuId = 9)
-- ============================================
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(9, NULL, N'Antiquités', 'antiquites', 1, 1, 1),
(9, NULL, N'Artistes & Musiciens', 'artistes-musiciens', 1, 2, 1),
(9, NULL, N'Billetterie', 'billetterie', 1, 3, 1),
(9, NULL, N'Collection', 'collection', 1, 4, 1),
(9, NULL, N'CD - Musique', 'cd-musique', 1, 5, 1),
(9, NULL, N'DVD - Films', 'dvd-films', 1, 6, 1),
(9, NULL, N'Instruments de musique', 'instruments-musique', 1, 7, 1),
(9, NULL, N'Livres', 'livres', 1, 8, 1),
(9, NULL, N'Modélisme', 'modelisme', 1, 9, 1),
(9, NULL, N'Vins & Gastronomie', 'vins-gastronomie', 1, 10, 1),
(9, NULL, N'Jeux & Jouets', 'jeux-jouets', 1, 11, 1),
(9, NULL, N'Loisirs créatifs', 'loisirs-creatifs', 1, 12, 1),
(9, NULL, N'Sport & Plein air', 'sport-plein-air', 1, 13, 1),
(9, NULL, N'Vélos', 'velos', 1, 14, 1),
(9, NULL, N'Équipements vélos', 'equipements-velos', 1, 15, 1);

-- ============================================
-- AUTRES (MenuId = 10)
-- ============================================
INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(10, NULL, N'Matériel professionnel', 'materiel-professionnel', 1, 1, 1),
(10, NULL, N'Services', 'services', 1, 2, 1),
(10, NULL, N'Animaux', 'animaux', 1, 3, 1),
(10, NULL, N'Dons', 'dons', 1, 4, 1),
(10, NULL, N'Autres', 'autres', 1, 5, 1);

INSERT INTO Categories (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder, IsActive) VALUES
(10, (SELECT Id FROM Categories WHERE MenuId=10 AND Name=N'Animaux'), N'Animaux', 'animaux/animaux', 1, 1, 1),
(10, (SELECT Id FROM Categories WHERE MenuId=10 AND Name=N'Animaux'), N'Accessoires animaux', 'animaux/accessoires', 1, 2, 1),
(10, (SELECT Id FROM Categories WHERE MenuId=10 AND Name=N'Animaux'), N'Animaux perdus', 'animaux/perdus', 1, 3, 1);

-- ============================================
-- INSERTION DES ANNONCES
-- ============================================
INSERT INTO Annonces (Title, Description, Price, Category, CreatedAt) VALUES
(N'Peugeot 308 2019 - Excellent état', N'Peugeot 308 année 2019, 45000 km, essence, boîte automatique. Première main, entretien concessionnaire.', 12500.00, N'Véhicules', '2024-06-01 10:30:00'),
(N'Renault Clio V - Comme neuve', N'Renault Clio V 2021, 20000 km, diesel, GPS intégré, caméra de recul.', 9800.00, N'Véhicules', '2024-06-01 11:00:00'),
(N'BMW Série 3 320d 2020', N'BMW 320d pack M, cuir, toit ouvrant, 60000 km, carnet entretien complet.', 28000.00, N'Véhicules', '2024-06-01 14:15:00'),
(N'Appartement S+2 Lac 2', N'Appartement S+2 au Lac 2, 85m², vue sur le lac, 2ème étage avec ascenseur, parking sous-sol.', 350000.00, N'Immobilier', '2024-06-02 09:00:00'),
(N'Villa avec jardin Soukra', N'Villa S+4 avec jardin 200m², piscine, garage double, quartier résidentiel calme.', 850000.00, N'Immobilier', '2024-06-02 10:30:00'),
(N'Studio meublé centre Tunis', N'Studio meublé 35m² au centre-ville, idéal étudiant ou jeune professionnel. Charges comprises.', 550.00, N'Immobilier', '2024-06-02 14:00:00'),
(N'iPhone 14 Pro 128Go', N'iPhone 14 Pro 128Go, couleur noir sidéral, état impeccable, batterie 92%. Avec boîte et accessoires.', 650.00, N'Électronique', '2024-06-03 08:45:00'),
(N'Samsung Galaxy S23 Ultra', N'Samsung Galaxy S23 Ultra 256Go, acheté neuf en janvier 2024, sous garantie.', 480.00, N'Électronique', '2024-06-03 09:20:00'),
(N'MacBook Pro M2 2023', N'MacBook Pro 14 pouces M2 Pro, 16Go RAM, 512Go SSD, comme neuf, peu utilisé.', 1200.00, N'Électronique', '2024-06-03 11:00:00'),
(N'Table basse scandinave en chêne', N'Table basse style scandinave en chêne massif, 120x60cm, très bon état.', 120.00, N'Maison & Jardin', '2024-06-03 15:00:00'),
(N'Canapé 3 places velours vert', N'Canapé 3 places en velours vert sapin, pieds dorés, acheté il y a 6 mois.', 580.00, N'Maison & Jardin', '2024-06-03 16:30:00'),
(N'Robe été fleurie taille M', N'Robe été légère motif fleuri, taille M, jamais portée, étiquette encore attachée.', 25.00, N'Mode', '2024-06-04 08:00:00'),
(N'Nike Air Max 90 neuves', N'Nike Air Max 90, taille 42, blanches, neuves dans leur boîte.', 85.00, N'Mode', '2024-06-04 09:15:00'),
(N'Montre Casio G-Shock', N'Montre Casio G-Shock modèle GA-2100, noire, neuve avec garantie 2 ans.', 75.00, N'Mode', '2024-06-04 10:30:00'),
(N'Poussette Yoyo Babyzen', N'Poussette Yoyo Babyzen, pliage compact, nacelle incluse, état parfait.', 280.00, N'Famille', '2024-06-04 14:00:00'),
(N'Vélo électrique Decathlon', N'VTT électrique Riverside 500E, batterie 400Wh, 500km parcourus.', 900.00, N'Loisirs', '2024-06-05 08:30:00'),
(N'Guitare acoustique Yamaha', N'Guitare acoustique Yamaha F310, parfaite pour débutant, avec housse.', 95.00, N'Loisirs', '2024-06-05 10:00:00'),
(N'PlayStation 5 + 2 manettes', N'PS5 version disque, 2 manettes DualSense, 3 jeux inclus.', 350.00, N'Électronique', '2024-06-05 11:45:00'),
(N'Développeur Web - CDI Tunis', N'Recherche développeur web Angular/Node.js, 3 ans expérience minimum. Salaire attractif.', 0.00, N'Emploi', '2024-06-05 14:00:00'),
(N'Location vacances Hammamet', N'Appartement S+1 vue mer à Hammamet, climatisé, piscine résidence, disponible juillet-août.', 150.00, N'Vacances', '2024-06-06 09:00:00');
