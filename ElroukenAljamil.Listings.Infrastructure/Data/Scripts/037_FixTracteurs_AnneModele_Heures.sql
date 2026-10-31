-- ============================================================
-- Fix : Tracteurs — Année modèle (number→select) + titre Heures
-- Idempotent
-- ============================================================

DECLARE @tracteurCatId INT;
DECLARE @workflowId    INT;
DECLARE @stepDetailsId INT;

SELECT @tracteurCatId = c.Id
FROM Categories c
JOIN Categories p ON c.ParentCategoryId = p.Id
WHERE c.Slug = 'tracteurs' AND p.Slug = 'materiel-professionnel';

SELECT @workflowId    = Id FROM DepositWorkflows WHERE CategoryId = @tracteurCatId;
SELECT @stepDetailsId = Id FROM WorkflowSteps    WHERE WorkflowId = @workflowId AND StepKey = 'details';

-- ── 1. Année modèle : number → select avec options 1950-2025 ─
UPDATE StepFields
SET
    FieldType   = 'select',
    Placeholder = N'Sélectionnez une année',
    Options     = N'[{"value":"2025","label":"2025"},{"value":"2024","label":"2024"},{"value":"2023","label":"2023"},{"value":"2022","label":"2022"},{"value":"2021","label":"2021"},{"value":"2020","label":"2020"},{"value":"2019","label":"2019"},{"value":"2018","label":"2018"},{"value":"2017","label":"2017"},{"value":"2016","label":"2016"},{"value":"2015","label":"2015"},{"value":"2014","label":"2014"},{"value":"2013","label":"2013"},{"value":"2012","label":"2012"},{"value":"2011","label":"2011"},{"value":"2010","label":"2010"},{"value":"2009","label":"2009"},{"value":"2008","label":"2008"},{"value":"2007","label":"2007"},{"value":"2006","label":"2006"},{"value":"2005","label":"2005"},{"value":"2004","label":"2004"},{"value":"2003","label":"2003"},{"value":"2002","label":"2002"},{"value":"2001","label":"2001"},{"value":"2000","label":"2000"},{"value":"1999","label":"1999"},{"value":"1998","label":"1998"},{"value":"1997","label":"1997"},{"value":"1996","label":"1996"},{"value":"1995","label":"1995"},{"value":"1994","label":"1994"},{"value":"1993","label":"1993"},{"value":"1992","label":"1992"},{"value":"1991","label":"1991"},{"value":"1990","label":"1990"},{"value":"1985","label":"1985"},{"value":"1980","label":"1980"},{"value":"1975","label":"1975"},{"value":"1970","label":"1970"},{"value":"1965","label":"1965"},{"value":"1960","label":"1960"},{"value":"1955","label":"1955"},{"value":"1950","label":"1950"}]',
    DefaultValue = '',
    ValidationRegex = ''
WHERE StepId = @stepDetailsId AND FieldKey = 'annee_modele';

-- ── 2. Ajouter section_title "Puissance" avant le champ puissance ─
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @stepDetailsId AND FieldKey = 'section_puissance')
BEGIN
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder,
                             Options, DefaultValue, Suffix, HelperText,
                             IsRequired, DisplayOrder, IsActive,
                             MaxLength, ValidationRegex, VisibilityCondition,
                             CreatedAt)
    VALUES (@stepDetailsId, 'section_puissance', N'Puissance',
            'section_title', '', '[]', '', '', '',
            0, 2, 1, NULL, '', '', GETUTCDATE());
END

-- ── 3. Ajouter section_title "Informations complémentaires" avant Heures ─
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @stepDetailsId AND FieldKey = 'section_heures')
BEGIN
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder,
                             Options, DefaultValue, Suffix, HelperText,
                             IsRequired, DisplayOrder, IsActive,
                             MaxLength, ValidationRegex, VisibilityCondition,
                             CreatedAt)
    VALUES (@stepDetailsId, 'section_heures', N'Informations complémentaires',
            'section_title', '', '[]', '', '', '',
            0, 5, 1, NULL, '', '', GETUTCDATE());
END

-- ── 4. Réordonner tous les champs ─
-- annee_modele(1) → section_puissance(2) → puissance(3) → marque(4) → section_heures(5) → heures(6)
UPDATE StepFields SET DisplayOrder = 1 WHERE StepId = @stepDetailsId AND FieldKey = 'annee_modele';
UPDATE StepFields SET DisplayOrder = 2 WHERE StepId = @stepDetailsId AND FieldKey = 'section_puissance';
UPDATE StepFields SET DisplayOrder = 3 WHERE StepId = @stepDetailsId AND FieldKey = 'puissance';
UPDATE StepFields SET DisplayOrder = 4 WHERE StepId = @stepDetailsId AND FieldKey = 'marque';
UPDATE StepFields SET DisplayOrder = 5 WHERE StepId = @stepDetailsId AND FieldKey = 'section_heures';
UPDATE StepFields SET DisplayOrder = 6 WHERE StepId = @stepDetailsId AND FieldKey = 'heures';

PRINT N'Fix Tracteurs terminé.';
