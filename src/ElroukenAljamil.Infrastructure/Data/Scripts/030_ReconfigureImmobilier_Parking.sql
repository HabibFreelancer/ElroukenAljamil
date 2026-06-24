-- ============================================
-- 030: Reconfigure Immobilier workflow - Parking form
--
-- Add dedicated parkingNature field visible only for "parking"
-- with options: Stationnement extérieur, Stationnement couvert,
--               Box ou garage fermé, Autre
--
-- Also remove parking from propertyNature visibility
-- (propertyNature has maison-oriented options, not relevant for parking)
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
-- 1. Add parkingNature field (order 14, same slot as terrainNature)
--    Both use order 14 — only one is visible at a time based on propertyType
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @StepId AND FieldKey = 'parkingNature')
BEGIN
    INSERT INTO StepFields
        (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue,
         Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex, VisibilityCondition)
    VALUES (
        @StepId,
        'parkingNature',
        N'Nature du bien',
        'select',
        N'Choisissez',
        N'[{"value":"exterieur","label":"Stationnement extérieur"},{"value":"couvert","label":"Stationnement couvert"},{"value":"box","label":"Box ou garage fermé"},{"value":"autre","label":"Autre"}]',
        '', '', '', 0, 14, 1, NULL, '',
        N'{"field":"propertyType","values":["parking"]}'
    );
END
ELSE
BEGIN
    UPDATE StepFields
    SET Label               = N'Nature du bien',
        FieldType           = 'select',
        Placeholder         = N'Choisissez',
        Options             = N'[{"value":"exterieur","label":"Stationnement extérieur"},{"value":"couvert","label":"Stationnement couvert"},{"value":"box","label":"Box ou garage fermé"},{"value":"autre","label":"Autre"}]',
        DefaultValue        = '',
        Suffix              = '',
        HelperText          = '',
        IsRequired          = 0,
        DisplayOrder        = 14,
        IsActive            = 1,
        MaxLength           = NULL,
        VisibilityCondition = N'{"field":"propertyType","values":["parking"]}'
    WHERE StepId = @StepId AND FieldKey = 'parkingNature';
END

-- ============================================================
-- 2. Remove parking from propertyNature visibility
--    propertyNature now only for maison + autre
-- ============================================================
UPDATE StepFields
SET VisibilityCondition = N'{"field":"propertyType","values":["maison","autre"]}'
WHERE StepId = @StepId AND FieldKey = 'propertyNature';

PRINT 'Script 030: parkingNature field added. propertyNature restricted to maison+autre.';
