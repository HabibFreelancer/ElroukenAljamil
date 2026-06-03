-- ============================================
-- ElroukenAljamil - Script SQL
-- Création des tables + Insertion des données
-- ============================================

-- Table Menu (menu principal du header)
CREATE TABLE Menu (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Slug NVARCHAR(100) NOT NULL,
    Icon NVARCHAR(100) NULL,
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Table Category (catégories / sous-menus dans le megamenu)
CREATE TABLE Category (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MenuId INT NOT NULL,
    ParentCategoryId INT NULL,
    Name NVARCHAR(200) NOT NULL,
    Slug NVARCHAR(200) NOT NULL,
    IsLink BIT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (MenuId) REFERENCES Menu(Id),
    FOREIGN KEY (ParentCategoryId) REFERENCES Category(Id)
);

-- ============================================
-- INSERTION DES MENUS
-- ============================================
INSERT INTO Menu (Name, Slug, Icon, DisplayOrder) VALUES
('Immobilier', 'immobilier', 'fa-solid fa-house', 1),
('Véhicules', 'vehicules', 'fa-solid fa-car', 2),
('Vacances', 'vacances', 'fa-solid fa-umbrella-beach', 3),
('Emploi', 'emploi', 'fa-solid fa-briefcase', 4),
('Mode', 'mode', 'fa-solid fa-shirt', 5),
('Maison & Jardin', 'maison-jardin', 'fa-solid fa-couch', 6),
('Famille', 'famille', 'fa-solid fa-baby', 7),
('Électronique', 'electronique', 'fa-solid fa-mobile-screen', 8),
('Loisirs', 'loisirs', 'fa-solid fa-futbol', 9),
('Autres', 'autre', 'fa-solid fa-ellipsis', 10);

-- ============================================
-- IMMOBILIER (MenuId = 1)
-- ============================================
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(1, NULL, 'Ventes immobilières', 'ventes', 1, 1),
(1, NULL, 'Immobilier Neuf', 'neuf', 1, 2),
(1, NULL, 'Locations', 'location', 1, 3),
(1, NULL, 'Colocations', 'colocation', 1, 4),
(1, NULL, 'Bureau & Commerce', 'bureau-commerce', 1, 5),
(1, NULL, 'Service de déménagement', 'demenagement', 1, 6);

-- Sous-catégories Ventes immobilières (ParentId = 1)
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(1, 1, 'Appartement', 'appartement', 1, 1),
(1, 1, 'Maison', 'maison', 1, 2),
(1, 1, 'Terrain', 'terrain', 1, 3);

-- Sous-catégories Immobilier Neuf (ParentId = 2)
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(1, 2, 'Appartement', 'neuf/appartement', 1, 1),
(1, 2, 'Maison', 'neuf/maison', 1, 2),
(1, 2, 'Programme logement neufs', 'neuf/programmes', 1, 3),
(1, 2, 'Promoteurs immobiliers', 'neuf/promoteurs', 1, 4);

-- Sous-catégories Locations (ParentId = 3)
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(1, 3, 'Appartement', 'location/appartement', 1, 1),
(1, 3, 'Maison', 'location/maison', 1, 2),
(1, 3, 'Parking', 'location/parking', 1, 3);

-- ============================================
-- VÉHICULES (MenuId = 2)
-- ============================================
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(2, NULL, 'Voitures', 'voitures', 1, 1),
(2, NULL, 'Motos', 'motos', 1, 2),
(2, NULL, 'Caravaning', 'caravaning', 1, 3),
(2, NULL, 'Utilitaires', 'utilitaires', 1, 4),
(2, NULL, 'Camions', 'camions', 1, 5),
(2, NULL, 'Nautisme', 'nautisme', 1, 6),
(2, NULL, 'Vélos', 'velos', 1, 7),
(2, NULL, 'Équipement auto', 'equipement-auto', 1, 8),
(2, NULL, 'Équipement moto', 'equipement-moto', 1, 9),
(2, NULL, 'Équipement caravaning', 'equipement-caravaning', 1, 10),
(2, NULL, 'Équipement nautisme', 'equipement-nautisme', 1, 11),
(2, NULL, 'Équipements vélos', 'equipements-velos', 1, 12),
(2, NULL, 'Services de réparations mécaniques', 'reparations', 1, 13);

-- Sous-catégories Voitures
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(2, (SELECT Id FROM Category WHERE MenuId=2 AND Name='Voitures'), 'Audi', 'voitures/audi', 1, 1),
(2, (SELECT Id FROM Category WHERE MenuId=2 AND Name='Voitures'), 'BMW', 'voitures/bmw', 1, 2),
(2, (SELECT Id FROM Category WHERE MenuId=2 AND Name='Voitures'), 'Mercedes', 'voitures/mercedes', 1, 3),
(2, (SELECT Id FROM Category WHERE MenuId=2 AND Name='Voitures'), 'Peugeot', 'voitures/peugeot', 1, 4),
(2, (SELECT Id FROM Category WHERE MenuId=2 AND Name='Voitures'), 'Renault', 'voitures/renault', 1, 5),
(2, (SELECT Id FROM Category WHERE MenuId=2 AND Name='Voitures'), 'Volkswagen', 'voitures/volkswagen', 1, 6);

-- Sous-catégories Motos
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(2, (SELECT Id FROM Category WHERE MenuId=2 AND Name='Motos'), 'BMW', 'motos/bmw', 1, 1),
(2, (SELECT Id FROM Category WHERE MenuId=2 AND Name='Motos'), 'Honda', 'motos/honda', 1, 2),
(2, (SELECT Id FROM Category WHERE MenuId=2 AND Name='Motos'), 'Kawasaki', 'motos/kawasaki', 1, 3),
(2, (SELECT Id FROM Category WHERE MenuId=2 AND Name='Motos'), 'Suzuki', 'motos/suzuki', 1, 4),
(2, (SELECT Id FROM Category WHERE MenuId=2 AND Name='Motos'), 'Yamaha', 'motos/yamaha', 1, 5);

-- ============================================
-- VACANCES (MenuId = 3)
-- ============================================
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(3, NULL, 'Types d''hébergements', 'types', 1, 1),
(3, NULL, 'Caractéristiques recherchées', 'caracteristiques', 0, 2),
(3, NULL, 'Nombre de voyageurs', 'voyageurs', 0, 3);

-- Sous-catégories Types d'hébergements
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(3, (SELECT Id FROM Category WHERE MenuId=3 AND Name='Types d''hébergements'), 'Maisons et villas', 'maisons-villas', 1, 1),
(3, (SELECT Id FROM Category WHERE MenuId=3 AND Name='Types d''hébergements'), 'Appartements', 'appartements', 1, 2),
(3, (SELECT Id FROM Category WHERE MenuId=3 AND Name='Types d''hébergements'), 'Chalets', 'chalets', 1, 3),
(3, (SELECT Id FROM Category WHERE MenuId=3 AND Name='Types d''hébergements'), 'Chambres d''hôtes', 'chambres-hotes', 1, 4),
(3, (SELECT Id FROM Category WHERE MenuId=3 AND Name='Types d''hébergements'), 'Campings', 'campings', 1, 5);

-- Sous-catégories Caractéristiques
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(3, (SELECT Id FROM Category WHERE MenuId=3 AND Name='Caractéristiques recherchées'), 'Piscine', 'piscine', 1, 1),
(3, (SELECT Id FROM Category WHERE MenuId=3 AND Name='Caractéristiques recherchées'), 'Jardin', 'jardin', 1, 2),
(3, (SELECT Id FROM Category WHERE MenuId=3 AND Name='Caractéristiques recherchées'), 'Animaux acceptés', 'animaux', 1, 3);

-- Sous-catégories Nombre de voyageurs
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(3, (SELECT Id FROM Category WHERE MenuId=3 AND Name='Nombre de voyageurs'), 'Solo', 'solo', 1, 1),
(3, (SELECT Id FROM Category WHERE MenuId=3 AND Name='Nombre de voyageurs'), 'À deux', 'a-deux', 1, 2),
(3, (SELECT Id FROM Category WHERE MenuId=3 AND Name='Nombre de voyageurs'), 'À quatre', 'a-quatre', 1, 3),
(3, (SELECT Id FROM Category WHERE MenuId=3 AND Name='Nombre de voyageurs'), 'À six', 'a-six', 1, 4),
(3, (SELECT Id FROM Category WHERE MenuId=3 AND Name='Nombre de voyageurs'), 'Plus de six', 'plus-de-six', 1, 5);

-- ============================================
-- EMPLOI (MenuId = 4)
-- ============================================
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(4, NULL, 'Offres d''emploi', 'offres', 1, 1),
(4, NULL, 'Formations professionnelles', 'formations', 1, 2),
(4, NULL, 'Profil Candidat', 'profil', 1, 3);

-- Sous-catégories Offres d'emploi
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(4, (SELECT Id FROM Category WHERE MenuId=4 AND Name='Offres d''emploi'), 'Intérim', 'interim', 1, 1),
(4, (SELECT Id FROM Category WHERE MenuId=4 AND Name='Offres d''emploi'), 'CDI', 'cdi', 1, 2),
(4, (SELECT Id FROM Category WHERE MenuId=4 AND Name='Offres d''emploi'), 'CDD', 'cdd', 1, 3),
(4, (SELECT Id FROM Category WHERE MenuId=4 AND Name='Offres d''emploi'), 'Bénévolat', 'benevolat', 1, 4),
(4, (SELECT Id FROM Category WHERE MenuId=4 AND Name='Offres d''emploi'), 'Autre', 'autre', 1, 5);

-- ============================================
-- MODE (MenuId = 5)
-- ============================================
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(5, NULL, 'Vêtements', 'vetements', 1, 1),
(5, NULL, 'Chaussures', 'chaussures', 1, 2),
(5, NULL, 'Montres & Bijoux', 'montres-bijoux', 1, 3),
(5, NULL, 'Accessoires & Bagagerie', 'accessoires', 1, 4);

-- Sous-catégories Vêtements
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Vêtements'), 'Femme', 'vetements/femme', 1, 1),
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Vêtements'), 'Maternité', 'vetements/maternite', 1, 2),
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Vêtements'), 'Homme', 'vetements/homme', 1, 3),
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Vêtements'), 'Enfant', 'vetements/enfant', 1, 4);

-- Sous-catégories Chaussures
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Chaussures'), 'Femme', 'chaussures/femme', 1, 1),
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Chaussures'), 'Homme', 'chaussures/homme', 1, 2),
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Chaussures'), 'Enfant', 'chaussures/enfant', 1, 3);

-- Sous-catégories Montres & Bijoux
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Montres & Bijoux'), 'Femme', 'montres-bijoux/femme', 1, 1),
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Montres & Bijoux'), 'Homme', 'montres-bijoux/homme', 1, 2),
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Montres & Bijoux'), 'Enfant', 'montres-bijoux/enfant', 1, 3),
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Montres & Bijoux'), 'Mixte', 'montres-bijoux/mixte', 1, 4);

-- Sous-catégories Accessoires & Bagagerie
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Accessoires & Bagagerie'), 'Femme', 'accessoires/femme', 1, 1),
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Accessoires & Bagagerie'), 'Homme', 'accessoires/homme', 1, 2),
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Accessoires & Bagagerie'), 'Enfant', 'accessoires/enfant', 1, 3),
(5, (SELECT Id FROM Category WHERE MenuId=5 AND Name='Accessoires & Bagagerie'), 'Mixte', 'accessoires/mixte', 1, 4);

-- ============================================
-- MAISON & JARDIN (MenuId = 6)
-- ============================================
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(6, NULL, 'Ameublement', 'ameublement', 1, 1),
(6, NULL, 'Papeterie & Fournitures scolaires', 'papeterie', 1, 2),
(6, NULL, 'Électroménager', 'electromenager', 1, 3),
(6, NULL, 'Arts de la table', 'arts-table', 1, 4),
(6, NULL, 'Décoration', 'decoration', 1, 5),
(6, NULL, 'Linge de maison', 'linge', 1, 6),
(6, NULL, 'Bricolage', 'bricolage', 1, 7),
(6, NULL, 'Jardin & Plantes', 'jardin-plantes', 1, 8),
(6, NULL, 'Services de jardinerie & bricolage', 'services', 1, 9);

-- Sous-catégories Ameublement
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Ameublement'), 'Armoire', 'ameublement/armoire', 1, 1),
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Ameublement'), 'Buffet', 'ameublement/buffet', 1, 2),
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Ameublement'), 'Canapé', 'ameublement/canape', 1, 3),
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Ameublement'), 'Chaise, tabouret et banc', 'ameublement/chaise', 1, 4),
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Ameublement'), 'Fauteuil', 'ameublement/fauteuil', 1, 5),
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Ameublement'), 'Lit', 'ameublement/lit', 1, 6),
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Ameublement'), 'Meuble de cuisine', 'ameublement/cuisine', 1, 7),
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Ameublement'), 'Table de salle à manger', 'ameublement/table', 1, 8);

-- Sous-catégories Électroménager
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Électroménager'), 'Aspirateur', 'electromenager/aspirateur', 1, 1),
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Électroménager'), 'Congélateur', 'electromenager/congelateur', 1, 2),
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Électroménager'), 'Four', 'electromenager/four', 1, 3),
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Électroménager'), 'Lave-linge', 'electromenager/lave-linge', 1, 4),
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Électroménager'), 'Lave-vaisselle', 'electromenager/lave-vaisselle', 1, 5),
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Électroménager'), 'Micro-ondes', 'electromenager/micro-ondes', 1, 6),
(6, (SELECT Id FROM Category WHERE MenuId=6 AND Name='Électroménager'), 'Réfrigérateur', 'electromenager/refrigerateur', 1, 7);

-- ============================================
-- FAMILLE (MenuId = 7)
-- ============================================
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(7, NULL, 'Équipement bébé', 'equipement-bebe', 1, 1),
(7, NULL, 'Mobilier enfant', 'mobilier-enfant', 1, 2),
(7, NULL, 'Vêtements bébé', 'vetements-bebe', 1, 3),
(7, NULL, 'Vêtements enfants', 'vetements-enfants', 1, 4),
(7, NULL, 'Vêtements maternité', 'vetements-maternite', 1, 5),
(7, NULL, 'Chaussures enfants', 'chaussures-enfants', 1, 6),
(7, NULL, 'Montres & bijoux enfants', 'montres-bijoux-enfants', 1, 7),
(7, NULL, 'Accessoires & bagagerie enfants', 'accessoires-enfants', 1, 8),
(7, NULL, 'Jeux & Jouets', 'jeux-jouets', 1, 9),
(7, NULL, 'Baby-Sitting', 'baby-sitting', 1, 10);

-- Sous-catégories Équipement bébé
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(7, (SELECT Id FROM Category WHERE MenuId=7 AND Name='Équipement bébé'), 'Poussette', 'equipement-bebe/poussette', 1, 1),
(7, (SELECT Id FROM Category WHERE MenuId=7 AND Name='Équipement bébé'), 'Siège auto', 'equipement-bebe/siege-auto', 1, 2);

-- Sous-catégories Vêtements bébé
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(7, (SELECT Id FROM Category WHERE MenuId=7 AND Name='Vêtements bébé'), '0 mois à 3 mois', 'vetements-bebe/0-3', 1, 1),
(7, (SELECT Id FROM Category WHERE MenuId=7 AND Name='Vêtements bébé'), '3 mois à 6 mois', 'vetements-bebe/3-6', 1, 2),
(7, (SELECT Id FROM Category WHERE MenuId=7 AND Name='Vêtements bébé'), '6 mois à 9 mois', 'vetements-bebe/6-9', 1, 3),
(7, (SELECT Id FROM Category WHERE MenuId=7 AND Name='Vêtements bébé'), '9 mois à 12 mois', 'vetements-bebe/9-12', 1, 4),
(7, (SELECT Id FROM Category WHERE MenuId=7 AND Name='Vêtements bébé'), '12 mois à 18 mois', 'vetements-bebe/12-18', 1, 5),
(7, (SELECT Id FROM Category WHERE MenuId=7 AND Name='Vêtements bébé'), '18 mois à 24 mois', 'vetements-bebe/18-24', 1, 6),
(7, (SELECT Id FROM Category WHERE MenuId=7 AND Name='Vêtements bébé'), 'Plus de 24 mois', 'vetements-bebe/24-plus', 1, 7);

-- ============================================
-- ÉLECTRONIQUE (MenuId = 8)
-- ============================================
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(8, NULL, 'Ordinateurs', 'ordinateurs', 1, 1),
(8, NULL, 'Accessoires informatique', 'accessoires-informatique', 1, 2),
(8, NULL, 'Tablettes & Liseuses', 'tablettes-liseuses', 1, 3),
(8, NULL, 'Photo, audio & vidéo', 'photo-audio-video', 1, 4),
(8, NULL, 'Téléphones & Objets connectés', 'telephones', 1, 5),
(8, NULL, 'Accessoires téléphone & Objets connectés', 'accessoires-telephone', 1, 6),
(8, NULL, 'Consoles', 'consoles', 1, 7),
(8, NULL, 'Jeux vidéo', 'jeux-video', 1, 8),
(8, NULL, 'Électroménager', 'electromenager', 1, 9),
(8, NULL, 'Services de réparations électroniques', 'reparations', 1, 10);

-- Sous-catégories Photo, audio & vidéo
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(8, (SELECT Id FROM Category WHERE MenuId=8 AND Name='Photo, audio & vidéo'), 'Télévision', 'photo-audio-video/television', 1, 1),
(8, (SELECT Id FROM Category WHERE MenuId=8 AND Name='Photo, audio & vidéo'), 'Enceintes', 'photo-audio-video/enceintes', 1, 2),
(8, (SELECT Id FROM Category WHERE MenuId=8 AND Name='Photo, audio & vidéo'), 'Appareil photo', 'photo-audio-video/appareil-photo', 1, 3),
(8, (SELECT Id FROM Category WHERE MenuId=8 AND Name='Photo, audio & vidéo'), 'Casque', 'photo-audio-video/casque', 1, 4),
(8, (SELECT Id FROM Category WHERE MenuId=8 AND Name='Photo, audio & vidéo'), 'Vidéoprojecteur', 'photo-audio-video/videoprojecteur', 1, 5),
(8, (SELECT Id FROM Category WHERE MenuId=8 AND Name='Photo, audio & vidéo'), 'Accessoires', 'photo-audio-video/accessoires', 1, 6),
(8, (SELECT Id FROM Category WHERE MenuId=8 AND Name='Photo, audio & vidéo'), 'Écouteurs', 'photo-audio-video/ecouteurs', 1, 7);

-- Sous-catégories Téléphones
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(8, (SELECT Id FROM Category WHERE MenuId=8 AND Name='Téléphones & Objets connectés'), 'Apple', 'telephones/apple', 1, 1),
(8, (SELECT Id FROM Category WHERE MenuId=8 AND Name='Téléphones & Objets connectés'), 'Samsung', 'telephones/samsung', 1, 2),
(8, (SELECT Id FROM Category WHERE MenuId=8 AND Name='Téléphones & Objets connectés'), 'Huawei', 'telephones/huawei', 1, 3),
(8, (SELECT Id FROM Category WHERE MenuId=8 AND Name='Téléphones & Objets connectés'), 'Sony', 'telephones/sony', 1, 4),
(8, (SELECT Id FROM Category WHERE MenuId=8 AND Name='Téléphones & Objets connectés'), 'One plus', 'telephones/oneplus', 1, 5),
(8, (SELECT Id FROM Category WHERE MenuId=8 AND Name='Téléphones & Objets connectés'), 'Google', 'telephones/google', 1, 6);

-- ============================================
-- LOISIRS (MenuId = 9)
-- ============================================
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(9, NULL, 'Antiquités', 'antiquites', 1, 1),
(9, NULL, 'Artistes & Musiciens', 'artistes-musiciens', 1, 2),
(9, NULL, 'Billetterie', 'billetterie', 1, 3),
(9, NULL, 'Collection', 'collection', 1, 4),
(9, NULL, 'CD - Musique', 'cd-musique', 1, 5),
(9, NULL, 'DVD - Films', 'dvd-films', 1, 6),
(9, NULL, 'Instruments de musique', 'instruments-musique', 1, 7),
(9, NULL, 'Livres', 'livres', 1, 8),
(9, NULL, 'Modélisme', 'modelisme', 1, 9),
(9, NULL, 'Vins & Gastronomie', 'vins-gastronomie', 1, 10),
(9, NULL, 'Jeux & Jouets', 'jeux-jouets', 1, 11),
(9, NULL, 'Loisirs créatifs', 'loisirs-creatifs', 1, 12),
(9, NULL, 'Sport & Plein air', 'sport-plein-air', 1, 13),
(9, NULL, 'Vélos', 'velos', 1, 14),
(9, NULL, 'Équipements vélos', 'equipements-velos', 1, 15);

-- Sous-catégories Jeux & Jouets
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(9, (SELECT Id FROM Category WHERE MenuId=9 AND Name='Jeux & Jouets'), 'Jeux de société', 'jeux-jouets/jeux-societe', 1, 1),
(9, (SELECT Id FROM Category WHERE MenuId=9 AND Name='Jeux & Jouets'), 'Poupées et accessoires', 'jeux-jouets/poupees', 1, 2),
(9, (SELECT Id FROM Category WHERE MenuId=9 AND Name='Jeux & Jouets'), 'Porteurs, trotteurs et draisiennes', 'jeux-jouets/porteurs', 1, 3),
(9, (SELECT Id FROM Category WHERE MenuId=9 AND Name='Jeux & Jouets'), 'Jouets d''éveil', 'jeux-jouets/eveil', 1, 4),
(9, (SELECT Id FROM Category WHERE MenuId=9 AND Name='Jeux & Jouets'), 'Cuisines et dînettes', 'jeux-jouets/cuisines', 1, 5),
(9, (SELECT Id FROM Category WHERE MenuId=9 AND Name='Jeux & Jouets'), 'Jeux de construction', 'jeux-jouets/construction', 1, 6),
(9, (SELECT Id FROM Category WHERE MenuId=9 AND Name='Jeux & Jouets'), 'Voitures et circuits', 'jeux-jouets/voitures-circuits', 1, 7),
(9, (SELECT Id FROM Category WHERE MenuId=9 AND Name='Jeux & Jouets'), 'Puzzle', 'jeux-jouets/puzzle', 1, 8);

-- Sous-catégories Vélos
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(9, (SELECT Id FROM Category WHERE MenuId=9 AND Name='Vélos'), 'Vélo de route', 'velos/route', 1, 1),
(9, (SELECT Id FROM Category WHERE MenuId=9 AND Name='Vélos'), 'VTT', 'velos/vtt', 1, 2),
(9, (SELECT Id FROM Category WHERE MenuId=9 AND Name='Vélos'), 'Vélo électrique', 'velos/electrique', 1, 3),
(9, (SELECT Id FROM Category WHERE MenuId=9 AND Name='Vélos'), 'Vélo enfant', 'velos/enfant', 1, 4),
(9, (SELECT Id FROM Category WHERE MenuId=9 AND Name='Vélos'), 'VTC', 'velos/vtc', 1, 5),
(9, (SELECT Id FROM Category WHERE MenuId=9 AND Name='Vélos'), 'Vélo de ville', 'velos/ville', 1, 6);

-- ============================================
-- AUTRES (MenuId = 10)
-- ============================================
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(10, NULL, 'Matériel professionnel', 'materiel-professionnel', 1, 1),
(10, NULL, 'Services', 'services', 1, 2),
(10, NULL, 'Animaux', 'animaux', 1, 3),
(10, NULL, 'Dons', 'dons', 1, 4),
(10, NULL, 'Autres', 'autres', 1, 5);

-- Sous-catégories Matériel professionnel
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Matériel professionnel'), 'Tracteurs', 'materiel-professionnel/tracteurs', 1, 1),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Matériel professionnel'), 'Matériel agricole', 'materiel-professionnel/agricole', 1, 2),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Matériel professionnel'), 'BTP - Chantier gros-oeuvre', 'materiel-professionnel/btp', 1, 3),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Matériel professionnel'), 'Poids lourds', 'materiel-professionnel/poids-lourds', 1, 4),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Matériel professionnel'), 'Manutention - Levage', 'materiel-professionnel/manutention', 1, 5),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Matériel professionnel'), 'Équipements industriels', 'materiel-professionnel/industriels', 1, 6),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Matériel professionnel'), 'Équipements pour restaurants & hôtels', 'materiel-professionnel/restaurants', 1, 7),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Matériel professionnel'), 'Équipements & Fournitures de bureau', 'materiel-professionnel/bureau', 1, 8),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Matériel professionnel'), 'Équipements pour commerces & marchés', 'materiel-professionnel/commerces', 1, 9),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Matériel professionnel'), 'Matériel médical', 'materiel-professionnel/medical', 1, 10);

-- Sous-catégories Services
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Services de déménagement', 'services/demenagement', 1, 1),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Services de réparations mécaniques', 'services/reparations-mecaniques', 1, 2),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Services de jardinerie & bricolage', 'services/jardinerie-bricolage', 1, 3),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Services à la personne', 'services/personne', 1, 4),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Services aux animaux', 'services/animaux', 1, 5),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Baby-Sitting', 'services/baby-sitting', 1, 6),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Artistes & Musiciens', 'services/artistes-musiciens', 1, 7),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Services évènementiels', 'services/evenementiels', 1, 8),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Services de réparations électroniques', 'services/reparations-electroniques', 1, 9),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Entraide entre voisins', 'services/entraide-voisins', 1, 10),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Billetterie', 'services/billetterie', 1, 11),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Évènements', 'services/evenements', 1, 12),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Covoiturage', 'services/covoiturage', 1, 13),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Cours particuliers', 'services/cours-particuliers', 1, 14),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Services'), 'Autres services', 'services/autres', 1, 15);

-- Sous-catégories Animaux
INSERT INTO Category (MenuId, ParentCategoryId, Name, Slug, IsLink, DisplayOrder) VALUES
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Animaux'), 'Animaux', 'animaux/animaux', 1, 1),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Animaux'), 'Accessoires animaux', 'animaux/accessoires', 1, 2),
(10, (SELECT Id FROM Category WHERE MenuId=10 AND Name='Animaux'), 'Animaux perdus', 'animaux/perdus', 1, 3);
