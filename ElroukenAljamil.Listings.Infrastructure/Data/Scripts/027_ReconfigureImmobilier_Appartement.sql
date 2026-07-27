-- ============================================
-- 027: Reconfigure Immobilier workflow - Appartement form
--
-- New order for Appartement:
--  2  surface
--  3  rooms
--  4  bedrooms
--  5  bathrooms
--  6  cuisine          (appt, maison, autre)
--  7  levels           (maison, autre)   — unchanged
--  8  condition        (état du bien)
--  9  floor            NEW - Étage de votre bien
--  10 totalFloors      NEW - Nombre d'étages dans l'immeuble
--  11 elevator         NEW - Ascenseur (toggle)
--  12 constructionYear (year AAAA)
--  13 section_atouts   (divider - appt + maison + autre)
--  14 propertyNature
--  15 features
--  16 landSurface
--  17 parking
--  18 heatingMode
--  19 exterior
--  20 exposure
--  21 availableFrom
-- ============================================

DECLARE @StepId INT = (
    SELECT s.Id FROM WorkflowSteps s
    INNER JOIN DepositWorkflows w ON w.Id = s.WorkflowId
    WHERE w.CategoryId = 1 AND s.StepKey = 'details'
);

IF @StepId IS NULL
BEGIN
    PRINT 'ERROR: Details step not found (CategoryId=1).';
    RETURN;
END

-- ============================================================
-- 1. Shift existing fields to make room for new ones
--    New slots: 9 (floor), 10 (totalFloors), 11 (elevator)
--    → push everything from order 9+ up by 3
-- ============================================================

-- cuisine stays at 6 (was 9 → bring closer, shared maison+appt)
UPDATE StepFields SET DisplayOrder = 6  WHERE StepId = @StepId AND FieldKey = 'cuisine';
-- levels stays at 7 (maison/autre only)
UPDATE StepFields SET DisplayOrder = 7  WHERE StepId = @StepId AND FieldKey = 'levels';
-- condition: order 8
UPDATE StepFields SET DisplayOrder = 8  WHERE StepId = @StepId AND FieldKey = 'condition';
-- constructionYear: order 12 (after elevator)
UPDATE StepFields SET DisplayOrder = 12 WHERE StepId = @StepId AND FieldKey = 'constructionYear';
-- section_atouts: order 13
UPDATE StepFields SET DisplayOrder = 13 WHERE StepId = @StepId AND FieldKey = 'section_atouts';
-- propertyNature: order 14
UPDATE StepFields SET DisplayOrder = 14 WHERE StepId = @StepId AND FieldKey = 'propertyNature';
-- features: order 15
UPDATE StepFields SET DisplayOrder = 15 WHERE StepId = @StepId AND FieldKey = 'features';
-- landSurface: order 16
UPDATE StepFields SET DisplayOrder = 16 WHERE StepId = @StepId AND FieldKey = 'landSurface';
-- parking: order 17
UPDATE StepFields SET DisplayOrder = 17 WHERE StepId = @StepId AND FieldKey = 'parking';
-- heatingMode: order 18
UPDATE StepFields SET DisplayOrder = 18 WHERE StepId = @StepId AND FieldKey = 'heatingMode';
-- exterior: order 19
UPDATE StepFields SET DisplayOrder = 19 WHERE StepId = @StepId AND FieldKey = 'exterior';
-- exposure: order 20
UPDATE StepFields SET DisplayOrder = 20 WHERE StepId = @StepId AND FieldKey = 'exposure';
-- availableFrom: order 21
UPDATE StepFields SET DisplayOrder = 21 WHERE StepId = @StepId AND FieldKey = 'availableFrom';

-- ============================================================
-- 2. cuisine: now visible for appartement too (was maison+autre)
-- ============================================================
UPDATE StepFields
SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'cuisine';

-- ============================================================
-- 3. section_atouts: now visible for appartement too
-- ============================================================
UPDATE StepFields
SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'section_atouts';

-- ============================================================
-- 4. Add new appartement-specific fields
-- ============================================================

-- floor: Étage de votre bien (number, appartement only, order 9)
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @StepId AND FieldKey = 'floor')
BEGIN
    INSERT INTO StepFields
        (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
         Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
    VALUES
        (@StepId, 'floor', N'Étage de votre bien', 'number', N'Ex: 3', '', '0',
         '', N'Indiquez 0 pour un rez-de-chaussée.', 0, 9, 1, NULL, '',
         N'{"field":"propertyType","values":["appartement"]}');
END
ELSE
BEGIN
    UPDATE StepFields
    SET Label              = N'Étage de votre bien',
        FieldType          = 'number',
        Placeholder        = N'Ex: 3',
        DefaultValue       = '0',
        Suffix             = '',
        HelperText         = N'Indiquez 0 pour un rez-de-chaussée.',
        IsRequired         = 0,
        DisplayOrder       = 9,
        IsActive           = 1,
        MaxLength          = NULL,
        VisibilityCondition = N'{"field":"propertyType","values":["appartement"]}'
    WHERE StepId = @StepId AND FieldKey = 'floor';
END

-- totalFloors: Nombre d'étages dans l'immeuble (number, appartement only, order 10)
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @StepId AND FieldKey = 'totalFloors')
BEGIN
    INSERT INTO StepFields
        (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
         Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
    VALUES
        (@StepId, 'totalFloors', N'Nombre d''étages dans l''immeuble', 'number', N'Ex: 5', '', '',
         '', '', 0, 10, 1, NULL, '',
         N'{"field":"propertyType","values":["appartement"]}');
END
ELSE
BEGIN
    UPDATE StepFields
    SET Label              = N'Nombre d''étages dans l''immeuble',
        FieldType          = 'number',
        Placeholder        = N'Ex: 5',
        DefaultValue       = '',
        Suffix             = '',
        HelperText         = '',
        IsRequired         = 0,
        DisplayOrder       = 10,
        IsActive           = 1,
        MaxLength          = NULL,
        VisibilityCondition = N'{"field":"propertyType","values":["appartement"]}'
    WHERE StepId = @StepId AND FieldKey = 'totalFloors';
END

-- elevator: Ascenseur (toggle, appartement only, order 11)
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @StepId AND FieldKey = 'elevator')
BEGIN
    INSERT INTO StepFields
        (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
         Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
    VALUES
        (@StepId, 'elevator', N'Ascenseur', 'toggle', '', '', 'false',
         '', '', 0, 11, 1, NULL, '',
         N'{"field":"propertyType","values":["appartement"]}');
END
ELSE
BEGIN
    UPDATE StepFields
    SET Label              = N'Ascenseur',
        FieldType          = 'toggle',
        Placeholder        = '',
        DefaultValue       = 'false',
        Suffix             = '',
        HelperText         = '',
        IsRequired         = 0,
        DisplayOrder       = 11,
        IsActive           = 1,
        MaxLength          = NULL,
        VisibilityCondition = N'{"field":"propertyType","values":["appartement"]}'
    WHERE StepId = @StepId AND FieldKey = 'elevator';
END

PRINT 'Script 027: Appartement fields (floor, totalFloors, elevator) added. Display orders updated.';
