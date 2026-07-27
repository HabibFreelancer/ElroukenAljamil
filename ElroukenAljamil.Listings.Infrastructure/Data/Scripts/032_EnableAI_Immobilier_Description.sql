-- ============================================
-- 032: Enable AI description generation for Immobilier workflow
--      Set HelperText = 'ai_enabled' on annonce_description field
--      in the description step of the Immobilier workflow
-- ============================================

DECLARE @WorkflowId INT = (
    SELECT Id FROM DepositWorkflows WHERE CategoryId = 1 AND IsActive = 1
);

DECLARE @StepDescId INT = (
    SELECT Id FROM WorkflowSteps
    WHERE WorkflowId = @WorkflowId AND StepKey = 'description'
);

IF @StepDescId IS NULL
BEGIN
    PRINT 'ERROR: Description step not found for Immobilier workflow.';
    RETURN;
END

UPDATE StepFields
SET HelperText = 'ai_enabled'
WHERE StepId = @StepDescId AND FieldKey = 'annonce_description';

PRINT 'Script 032: AI generation enabled for Immobilier annonce_description field.';
