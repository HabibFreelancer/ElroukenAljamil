-- Add Price step between details (3) and description (4)
-- First reorder existing steps
UPDATE WorkflowSteps SET StepOrder = 5 WHERE Id = 18; -- description -> 5
UPDATE WorkflowSteps SET StepOrder = 6 WHERE Id = 19; -- location -> 6
UPDATE WorkflowSteps SET StepOrder = 7 WHERE Id = 20; -- contact -> 7

-- Insert price step at order 4
INSERT INTO WorkflowSteps (WorkflowId, StepOrder, Title, Subtitle, StepKey, IsRequired, IsActive)
VALUES (3, 4, N'Quel est votre prix ?', N'Indiquez le prix de vente de votre vehicule.', 'price', 1, 1);

DECLARE @StepPrice INT = SCOPE_IDENTITY();

-- Price field
INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
VALUES (@StepPrice, 'price', N'Votre prix de vente', 'price_gauge', N'Ex: 15000', '', '', 'TND', '', 1, 1, 1, NULL, '');

-- Remove price from description step (it was at StepId=18)
DELETE FROM StepFields WHERE StepId = 18 AND FieldKey = 'price';

PRINT 'Price step added at order 4. Steps reordered.';
GO

-- ============================================
-- Seed test data: Annonces for Voitures category (ID=17)
-- These will be used for "similar ads" and price gauge
-- ============================================
INSERT INTO Annonces (Title, Description, Price, CategoryId, AdType, [Condition], Location, Phone, Email, HidePhone, ExtraData, CreatedAt) VALUES
(N'Citroen C5 Aircross 2022 Essence Automatique', N'SUV familial en excellent etat', 65000, 17, N'Vente', N'Tres bon etat', N'Tunis', N'55000001', N'test1@test.com', 0, N'{"brand":"citroen","model":"c5_aircross","year":"2022","fuel":"essence","mileage":"13258","gearbox":"automatique"}', GETUTCDATE()),
(N'Citroen C5 Aircross 2021 Diesel', N'Faible kilometrage', 60000, 17, N'Vente', N'Bon etat', N'Sousse', N'55000002', N'test2@test.com', 0, N'{"brand":"citroen","model":"c5_aircross","year":"2021","fuel":"diesel","mileage":"24121","gearbox":"automatique"}', GETUTCDATE()),
(N'Citroen C5 Aircross 2022 Hybride', N'Version hybride rechargeable', 72000, 17, N'Vente', N'Neuf', N'Sfax', N'55000003', N'test3@test.com', 0, N'{"brand":"citroen","model":"c5_aircross","year":"2022","fuel":"hybride","mileage":"8500","gearbox":"automatique"}', GETUTCDATE()),
(N'Peugeot 3008 2021 Diesel', N'SUV premium', 58000, 17, N'Vente', N'Tres bon etat', N'Tunis', N'55000004', N'test4@test.com', 0, N'{"brand":"peugeot","model":"3008","year":"2021","fuel":"diesel","mileage":"35000","gearbox":"automatique"}', GETUTCDATE()),
(N'Peugeot 3008 2022 Essence', N'Comme neuf', 62000, 17, N'Vente', N'Tres bon etat', N'Bizerte', N'55000005', N'test5@test.com', 0, N'{"brand":"peugeot","model":"3008","year":"2022","fuel":"essence","mileage":"15000","gearbox":"manuelle"}', GETUTCDATE()),
(N'Volkswagen Tiguan 2020 Diesel', N'Entretien suivi', 55000, 17, N'Vente', N'Bon etat', N'Nabeul', N'55000006', N'test6@test.com', 0, N'{"brand":"volkswagen","model":"tiguan","year":"2020","fuel":"diesel","mileage":"45000","gearbox":"automatique"}', GETUTCDATE()),
(N'Renault Captur 2022 Essence', N'Citadine SUV', 42000, 17, N'Vente', N'Tres bon etat', N'Tunis', N'55000007', N'test7@test.com', 0, N'{"brand":"renault","model":"captur","year":"2022","fuel":"essence","mileage":"18000","gearbox":"manuelle"}', GETUTCDATE()),
(N'Toyota RAV4 2021 Hybride', N'SUV hybride', 85000, 17, N'Vente', N'Tres bon etat', N'Tunis', N'55000008', N'test8@test.com', 0, N'{"brand":"toyota","model":"rav4","year":"2021","fuel":"hybride","mileage":"22000","gearbox":"automatique"}', GETUTCDATE()),
(N'BMW X3 2020 Diesel', N'Premium SUV', 95000, 17, N'Vente', N'Bon etat', N'Tunis', N'55000009', N'test9@test.com', 0, N'{"brand":"bmw","model":"x3","year":"2020","fuel":"diesel","mileage":"40000","gearbox":"automatique"}', GETUTCDATE()),
(N'Dacia Duster 2022 Diesel', N'SUV economique', 38000, 17, N'Vente', N'Neuf', N'Sousse', N'55000010', N'test10@test.com', 0, N'{"brand":"dacia","model":"duster","year":"2022","fuel":"diesel","mileage":"5000","gearbox":"manuelle"}', GETUTCDATE());

PRINT '10 test annonces inserted for price gauge and similar ads.';
