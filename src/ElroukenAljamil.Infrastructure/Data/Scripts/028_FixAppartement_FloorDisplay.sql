-- ============================================
-- 028: Fix display of floor and totalFloors fields
--      Add suffix for compact display with input-with-suffix style
-- ============================================

DECLARE @StepId INT = (
    SELECT s.Id FROM WorkflowSteps s
    INNER JOIN DepositWorkflows w ON w.Id = s.WorkflowId
    WHERE w.CategoryId = 1 AND s.StepKey = 'details'
);

-- Étage de votre bien: suffix "ème étage"
UPDATE StepFields
SET Suffix = N'ème étage'
WHERE StepId = @StepId AND FieldKey = 'floor';

-- Nombre d'étages dans l'immeuble: suffix "étage(s)"
UPDATE StepFields
SET Suffix = N'étage(s)'
WHERE StepId = @StepId AND FieldKey = 'totalFloors';

PRINT 'Script 028: floor and totalFloors suffix updated.';
