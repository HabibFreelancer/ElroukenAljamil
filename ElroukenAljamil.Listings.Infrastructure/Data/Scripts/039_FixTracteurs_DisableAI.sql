-- ============================================================
-- Fix : Tracteurs — Désactiver génération IA description
-- Idempotent
-- ============================================================

DECLARE @tracteurCatId INT;
DECLARE @workflowId    INT;
DECLARE @stepDescId    INT;

SELECT @tracteurCatId = c.Id
FROM Categories c
JOIN Categories p ON c.ParentCategoryId = p.Id
WHERE c.Slug = 'tracteurs' AND p.Slug = 'materiel-professionnel';

SELECT @workflowId = Id FROM DepositWorkflows WHERE CategoryId = @tracteurCatId;
SELECT @stepDescId = Id FROM WorkflowSteps WHERE WorkflowId = @workflowId AND StepKey = 'description';

UPDATE StepFields
SET HelperText = ''
WHERE StepId = @stepDescId AND FieldKey = 'annonce_description';

PRINT N'IA désactivée pour Tracteurs description.';
