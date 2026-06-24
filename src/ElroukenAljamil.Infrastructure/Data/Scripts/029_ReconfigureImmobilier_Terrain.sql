-- ============================================
-- 029: Reconfigure Immobilier workflow - Terrain form
--
-- propertyNature is shared with maison/parking/autre and has house-oriented options.
-- For terrain we add a dedicated field: terrainNature
-- visible only for "terrain", with options:
--   Jardin, Terrain constructible, Terrain agricole, Autre
--
-- Terrain form fields (visible for "terrain"):
--   2  surface (Surface habitable → relabeled to Surface du terrain for terrain)
--   12 constructionYear → NOT visible for terrain
--   14 terrainNature (NEW)
--   16 landSurface
--   21 availableFrom
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
-- 1. Add terrainNature field (order 14, between section_atouts=13 and features=15)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @StepId AND FieldKey = 'terrainNature')
BEGIN
    INSERT INTO StepFields
        (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
         Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
    VALUES (
        @StepId,
        'terrainNature',
        N'Nature du terrain',
        'select',
        N'Choisissez',
        N'[{"value":"jardin","label":"Jardin"},{"value":"constructible","label":"Terrain constructible"},{"value":"agricole","label":"Terrain agricole"},{"value":"autre","label":"Autre"}]',
        '', '', '', 0, 14, 1, NULL, '',
        N'{"field":"propertyType","values":["terrain"]}'
    );
END
ELSE
BEGIN
    UPDATE StepFields
    SET Label               = N'Nature du terrain',
        FieldType           = 'select',
        Placeholder         = N'Choisissez',
        Options             = N'[{"value":"jardin","label":"Jardin"},{"value":"constructible","label":"Terrain constructible"},{"value":"agricole","label":"Terrain agricole"},{"value":"autre","label":"Autre"}]',
        DefaultValue        = '',
        Suffix              = '',
        HelperText          = '',
        IsRequired          = 0,
        DisplayOrder        = 14,
        IsActive            = 1,
        MaxLength           = NULL,
        VisibilityCondition = N'{"field":"propertyType","values":["terrain"]}'
    WHERE StepId = @StepId AND FieldKey = 'terrainNature';
END

-- ============================================================
-- 2. Remove terrain from propertyNature visibility
--    (propertyNature has maison-oriented options, not relevant for terrain)
-- ============================================================
UPDATE StepFields
SET VisibilityCondition = N'{"field":"propertyType","values":["maison","parking","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'propertyNature';

-- ============================================================
-- 3. constructionYear: remove terrain from visibility
--    (no construction year for a plot of land)
-- ============================================================
UPDATE StepFields
SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","parking","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'constructionYear';

-- ============================================================
-- 4. surface: terrain should show surface too (already visible — confirm)
--    Keep surface visible for terrain via existing condition (maison, appt, autre)
--    → extend to include terrain
-- ============================================================
UPDATE StepFields
SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","terrain","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'surface';

-- landSurface: keep visible for terrain
-- already: maison, terrain, autre — confirmed correct

PRINT 'Script 029: terrainNature field added. propertyNature and constructionYear visibility updated.';
