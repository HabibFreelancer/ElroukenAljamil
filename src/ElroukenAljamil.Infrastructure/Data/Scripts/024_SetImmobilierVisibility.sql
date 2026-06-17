-- Set visibility conditions for Immobilier workflow details fields
DECLARE @StepId INT = (SELECT Id FROM WorkflowSteps WHERE WorkflowId = (SELECT Id FROM DepositWorkflows WHERE CategoryId = 1) AND StepKey = 'details');

-- propertyType: always visible (no condition)
-- surface: maison, appartement, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","autre"]}' WHERE StepId = @StepId AND FieldKey = 'surface';
-- rooms: maison, appartement, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","autre"]}' WHERE StepId = @StepId AND FieldKey = 'rooms';
-- bedrooms: maison, appartement, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","autre"]}' WHERE StepId = @StepId AND FieldKey = 'bedrooms';
-- bathrooms: maison, appartement, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","autre"]}' WHERE StepId = @StepId AND FieldKey = 'bathrooms';
-- levels: maison, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","autre"]}' WHERE StepId = @StepId AND FieldKey = 'levels';
-- constructionYear: maison, appartement, parking, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","parking","autre"]}' WHERE StepId = @StepId AND FieldKey = 'constructionYear';
-- propertyNature: maison, terrain, parking, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","terrain","parking","autre"]}' WHERE StepId = @StepId AND FieldKey = 'propertyNature';
-- condition: appartement, parking
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["appartement","parking"]}' WHERE StepId = @StepId AND FieldKey = 'condition';
-- features: maison, appartement, parking, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","parking","autre"]}' WHERE StepId = @StepId AND FieldKey = 'features';
-- landSurface: maison, terrain, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","terrain","autre"]}' WHERE StepId = @StepId AND FieldKey = 'landSurface';
-- parking: maison, appartement, parking, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","parking","autre"]}' WHERE StepId = @StepId AND FieldKey = 'parking';
-- heatingMode: maison, appartement, autre
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement","autre"]}' WHERE StepId = @StepId AND FieldKey = 'heatingMode';
-- exterior: appartement
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["appartement"]}' WHERE StepId = @StepId AND FieldKey = 'exterior';
-- exposure: appartement
UPDATE StepFields SET VisibilityCondition = N'{"field":"propertyType","values":["appartement"]}' WHERE StepId = @StepId AND FieldKey = 'exposure';
-- availableFrom: always visible (all types)

PRINT 'Visibility conditions set for 14 fields.';
