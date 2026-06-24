-- ============================================
-- 026: Reconfigure Immobilier workflow - Details step
--   - propertyType: Maison, Appartement, Terrain, Parking, Autre (no Villa)
--   - Maison form: add cuisine, fix levels/constructionYear, add condition,
--     section divider, propertyNature multiselect, exposure after parking
--   - Full visibility conditions reset
-- ============================================

DECLARE @StepId INT = (
    SELECT s.Id FROM WorkflowSteps s
    INNER JOIN DepositWorkflows w ON w.Id = s.WorkflowId
    WHERE w.CategoryId = 1 AND s.StepKey = 'details'
);

IF @StepId IS NULL
BEGIN
    PRINT 'ERROR: Details step not found for Immobilier workflow (CategoryId=1).';
    RETURN;
END

-- ============================================================
-- 1. Fix propertyType: Maison, Appartement, Terrain, Parking, Autre
-- ============================================================
UPDATE StepFields
SET Options = N'[{"value":"maison","label":"Maison"},{"value":"appartement","label":"Appartement"},{"value":"terrain","label":"Terrain"},{"value":"parking","label":"Parking"},{"value":"autre","label":"Autre"}]'
WHERE StepId = @StepId AND FieldKey = 'propertyType';

-- ============================================================
-- 2. Fix existing fields
-- ============================================================

-- surface: label + helperText correct, keep as-is (number + m2 suffix)

-- rooms: keep as-is (number + piece(s) suffix)

-- bedrooms: keep as-is (select)

-- bathrooms: keep as-is (select, optional)

-- levels: fix display - add suffix 'niveau(x)' + fix helperText
UPDATE StepFields
SET
    Label        = N'Nombre de niveaux',
    FieldType    = 'number',
    Placeholder  = N'Ex: 2',
    Suffix       = N'niveau(x)',
    HelperText   = N'Indiquez 1 pour une maison de plain-pied.',
    IsRequired   = 0,
    DisplayOrder = 6
WHERE StepId = @StepId AND FieldKey = 'levels';

-- constructionYear: change to year type (4-digit), MaxLength=4
UPDATE StepFields
SET
    Label        = N'Année de construction',
    FieldType    = 'year',
    Placeholder  = N'AAAA',
    Suffix       = '',
    HelperText   = '',
    MaxLength    = 4,
    IsRequired   = 0,
    DisplayOrder = 7
WHERE StepId = @StepId AND FieldKey = 'constructionYear';

-- condition (état du bien): move to display order 8, visible for maison + appartement + parking + autre
-- Update label and options to proper French
UPDATE StepFields
SET
    Label        = N'État du bien',
    FieldType    = 'select',
    Options      = N'[{"value":"tres_bon","label":"Très bon état"},{"value":"bon","label":"Bon état"},{"value":"renove","label":"Rénové"},{"value":"rafraichir","label":"À rafraîchir"},{"value":"travaux","label":"Travaux à prévoir"}]',
    Placeholder  = N'Choisissez',
    IsRequired   = 0,
    DisplayOrder = 8
WHERE StepId = @StepId AND FieldKey = 'condition';

-- propertyNature: change to multiselect, display order 10 (after section divider at 9)
UPDATE StepFields
SET
    Label        = N'Nature du bien',
    FieldType    = 'multiselect',
    Options      = N'[{"value":"villa","label":"Villa"},{"value":"individuelle","label":"Maison individuelle"},{"value":"ville","label":"Maison de ville"},{"value":"plain_pied","label":"Maison de plain-pied"},{"value":"ferme","label":"Ferme"},{"value":"mitoyenne","label":"Maison mitoyenne"},{"value":"autre","label":"Autre"}]',
    Placeholder  = N'',
    IsRequired   = 0,
    DisplayOrder = 10
WHERE StepId = @StepId AND FieldKey = 'propertyNature';

-- features: display order 11
UPDATE StepFields SET DisplayOrder = 11 WHERE StepId = @StepId AND FieldKey = 'features';

-- landSurface: display order 12
UPDATE StepFields SET DisplayOrder = 12 WHERE StepId = @StepId AND FieldKey = 'landSurface';

-- parking (places): display order 13
UPDATE StepFields SET DisplayOrder = 13 WHERE StepId = @StepId AND FieldKey = 'parking';

-- heatingMode: display order 14
UPDATE StepFields SET DisplayOrder = 14 WHERE StepId = @StepId AND FieldKey = 'heatingMode';

-- exterior: display order 15
UPDATE StepFields SET DisplayOrder = 15 WHERE StepId = @StepId AND FieldKey = 'exterior';

-- exposure: move to display order 16 (after parking, before availableFrom)
UPDATE StepFields
SET
    Label        = N'Exposition',
    FieldType    = 'select',
    Options      = N'[{"value":"nord","label":"Nord"},{"value":"sud","label":"Sud"},{"value":"est","label":"Est"},{"value":"ouest","label":"Ouest"},{"value":"nord_est","label":"Nord-Est"},{"value":"nord_ouest","label":"Nord-Ouest"},{"value":"sud_est","label":"Sud-Est"},{"value":"sud_ouest","label":"Sud-Ouest"}]',
    Placeholder  = N'Choisissez',
    IsRequired   = 0,
    DisplayOrder = 16
WHERE StepId = @StepId AND FieldKey = 'exposure';

-- availableFrom: display order 17
UPDATE StepFields SET DisplayOrder = 17 WHERE StepId = @StepId AND FieldKey = 'availableFrom';

-- ============================================================
-- 3. Add new fields (cuisine + section_atouts divider)
-- ============================================================

-- cuisine: multiselect, display order 9 (before propertyNature, in "Les critères indispensables")
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @StepId AND FieldKey = 'cuisine')
BEGIN
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (
        @StepId,
        'cuisine',
        N'Cuisine',
        'multiselect',
        '',
        N'[{"value":"equipee","label":"Équipée"},{"value":"ouverte","label":"Ouverte"},{"value":"separee","label":"Séparée"}]',
        '', '', '', 0, 9, 1, NULL, ''
    );
END
ELSE
BEGIN
    UPDATE StepFields
    SET
        Label        = N'Cuisine',
        FieldType    = 'multiselect',
        Options      = N'[{"value":"equipee","label":"Équipée"},{"value":"ouverte","label":"Ouverte"},{"value":"separee","label":"Séparée"}]',
        DisplayOrder = 9,
        IsActive     = 1
    WHERE StepId = @StepId AND FieldKey = 'cuisine';
END

-- section_atouts: special field used as a visual section divider in the frontend
-- FieldType = 'section_title', no data stored, display order 9.5 → use 95 (between cuisine=9 and propertyNature=10)
-- We shift propertyNature to 11 to make room
UPDATE StepFields SET DisplayOrder = 11 WHERE StepId = @StepId AND FieldKey = 'propertyNature';
UPDATE StepFields SET DisplayOrder = 12 WHERE StepId = @StepId AND FieldKey = 'features';
UPDATE StepFields SET DisplayOrder = 13 WHERE StepId = @StepId AND FieldKey = 'landSurface';
UPDATE StepFields SET DisplayOrder = 14 WHERE StepId = @StepId AND FieldKey = 'parking';
UPDATE StepFields SET DisplayOrder = 15 WHERE StepId = @StepId AND FieldKey = 'heatingMode';
UPDATE StepFields SET DisplayOrder = 16 WHERE StepId = @StepId AND FieldKey = 'exterior';
UPDATE StepFields SET DisplayOrder = 17 WHERE StepId = @StepId AND FieldKey = 'exposure';
UPDATE StepFields SET DisplayOrder = 18 WHERE StepId = @StepId AND FieldKey = 'availableFrom';

-- section_atouts divider at display order 10
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @StepId AND FieldKey = 'section_atouts')
BEGIN
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (
        @StepId,
        'section_atouts',
        N'Les atouts qui font la différence',
        'section_title',
        '', '', '', '', '', 0, 10, 1, NULL, ''
    );
END
ELSE
BEGIN
    UPDATE StepFields
    SET
        Label        = N'Les atouts qui font la différence',
        FieldType    = 'section_title',
        DisplayOrder = 10,
        IsActive     = 1
    WHERE StepId = @StepId AND FieldKey = 'section_atouts';
END

-- ============================================================
-- 4. Reset all visibility conditions for maison form
-- ============================================================

-- surface: maison, appartement, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'surface';

-- rooms: maison, appartement, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'rooms';

-- bedrooms: maison, appartement, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'bedrooms';

-- bathrooms: maison, appartement, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'bathrooms';

-- cuisine: maison, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'cuisine';

-- levels: maison, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'levels';

-- constructionYear: maison, appartement, parking, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","parking","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'constructionYear';

-- condition: maison, appartement, parking, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","parking","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'condition';

-- section_atouts: maison, autre (same as its children)
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'section_atouts';

-- propertyNature: maison, terrain, parking, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","terrain","parking","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'propertyNature';

-- features: maison, appartement, parking, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","parking","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'features';

-- landSurface: maison, terrain, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","terrain","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'landSurface';

-- parking: maison, appartement, parking, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","parking","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'parking';

-- heatingMode: maison, appartement, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'heatingMode';

-- exterior: appartement, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["appartement","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'exterior';

-- exposure: maison, appartement, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'exposure';

-- availableFrom: all types (no condition)
UPDATE StepFields SET VisibilityCondition = ''
WHERE StepId = @StepId AND FieldKey = 'availableFrom';

PRINT 'Script 026: Immobilier details step fully reconfigured for Maison form.';
