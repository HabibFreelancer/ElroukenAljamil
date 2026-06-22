-- ============================================
-- 025: Update Immobilier workflow
--      - Add "Villa" as a propertyType option
--      - Update visibility conditions to include "villa"
-- ============================================

DECLARE @StepId INT = (
    SELECT s.Id FROM WorkflowSteps s
    INNER JOIN DepositWorkflows w ON w.Id = s.WorkflowId
    WHERE w.CategoryId = 1 AND s.StepKey = 'details'
);

IF @StepId IS NOT NULL
BEGIN
    -- Update propertyType pills to include "villa"
    UPDATE StepFields
    SET Options = N'[{"value":"maison","label":"Maison","icon":"fa-solid fa-house"},{"value":"appartement","label":"Appartement","icon":"fa-solid fa-building"},{"value":"villa","label":"Villa","icon":"fa-solid fa-house-chimney"},{"value":"autre","label":"Autre","icon":"fa-solid fa-ellipsis"}]'
    WHERE StepId = @StepId AND FieldKey = 'propertyType';

    -- surface: maison, appartement, villa, autre
    UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","villa","autre"]}'
    WHERE StepId = @StepId AND FieldKey = 'surface';

    -- rooms: maison, appartement, villa, autre
    UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","villa","autre"]}'
    WHERE StepId = @StepId AND FieldKey = 'rooms';

    -- bedrooms: maison, appartement, villa, autre
    UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","villa","autre"]}'
    WHERE StepId = @StepId AND FieldKey = 'bedrooms';

    -- bathrooms: maison, appartement, villa, autre
    UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","villa","autre"]}'
    WHERE StepId = @StepId AND FieldKey = 'bathrooms';

    -- levels: maison, villa, autre
    UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","villa","autre"]}'
    WHERE StepId = @StepId AND FieldKey = 'levels';

    -- constructionYear: maison, appartement, villa, parking, autre
    UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","villa","parking","autre"]}'
    WHERE StepId = @StepId AND FieldKey = 'constructionYear';

    -- propertyNature: maison, villa, terrain, parking, autre
    UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","villa","terrain","parking","autre"]}'
    WHERE StepId = @StepId AND FieldKey = 'propertyNature';

    -- condition: appartement, parking
    UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["appartement","parking"]}'
    WHERE StepId = @StepId AND FieldKey = 'condition';

    -- features: maison, appartement, villa, parking, autre
    UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","villa","parking","autre"]}'
    WHERE StepId = @StepId AND FieldKey = 'features';

    -- landSurface: maison, villa, terrain, autre
    UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","villa","terrain","autre"]}'
    WHERE StepId = @StepId AND FieldKey = 'landSurface';

    -- parking: maison, appartement, villa, parking, autre
    UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","villa","parking","autre"]}'
    WHERE StepId = @StepId AND FieldKey = 'parking';

    -- heatingMode: maison, appartement, villa, autre
    UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","villa","autre"]}'
    WHERE StepId = @StepId AND FieldKey = 'heatingMode';

    -- exterior: appartement, villa
    UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["appartement","villa"]}'
    WHERE StepId = @StepId AND FieldKey = 'exterior';

    -- exposure: appartement, villa
    UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["appartement","villa"]}'
    WHERE StepId = @StepId AND FieldKey = 'exposure';

    PRINT 'Immobilier propertyType updated with Villa + visibility conditions refreshed.';
END
ELSE
    PRINT 'Details step not found for Immobilier workflow (CategoryId=1).';
