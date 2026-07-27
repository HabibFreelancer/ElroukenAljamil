-- ============================================
-- 031: Reconfigure Immobilier workflow - Autre form
--      Remove: cuisine, levels, propertyNature, exposure from "autre"
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

-- cuisine: maison only (remove autre)
UPDATE StepFields
SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement"]}'
WHERE StepId = @StepId AND FieldKey = 'cuisine';

-- levels: maison only (remove autre)
UPDATE StepFields
SET VisibilityCondition = N'{"field":"propertyType","values":["maison"]}'
WHERE StepId = @StepId AND FieldKey = 'levels';

-- propertyNature: maison only (remove autre — was maison+autre)
UPDATE StepFields
SET VisibilityCondition = N'{"field":"propertyType","values":["maison"]}'
WHERE StepId = @StepId AND FieldKey = 'propertyNature';

-- exposure: maison, appartement only (remove autre)
UPDATE StepFields
SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement"]}'
WHERE StepId = @StepId AND FieldKey = 'exposure';

-- section_atouts: maison, appartement only (remove autre — follows its children)
UPDATE StepFields
SET VisibilityCondition = N'{"field":"propertyType","values":["maison","appartement"]}'
WHERE StepId = @StepId AND FieldKey = 'section_atouts';

PRINT 'Script 031: cuisine, levels, propertyNature, exposure removed from "autre" type.';
