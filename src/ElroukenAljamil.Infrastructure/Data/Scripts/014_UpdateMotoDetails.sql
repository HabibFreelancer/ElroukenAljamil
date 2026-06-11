DECLARE @StepId INT = (SELECT Id FROM WorkflowSteps WHERE WorkflowId = 4 AND StepKey = 'details');

-- Update model to dependent_select
UPDATE StepFields SET FieldType = 'dependent_select', Placeholder = N'Choisissez' WHERE StepId = @StepId AND FieldKey = 'model';

-- Update brands list
UPDATE StepFields SET IsRequired = 1, Options = N'[{"value":"aprilia","label":"Aprilia"},{"value":"bmw","label":"BMW"},{"value":"benelli","label":"Benelli"},{"value":"bimota","label":"Bimota"},{"value":"bsa","label":"BSA"},{"value":"cagiva","label":"Cagiva"},{"value":"daelim","label":"Daelim"},{"value":"derbi","label":"Derbi"},{"value":"ducati","label":"Ducati"},{"value":"gasgas","label":"GasGas"},{"value":"harley","label":"Harley-Davidson"},{"value":"honda","label":"Honda"},{"value":"husaberg","label":"Husaberg"},{"value":"husqvarna","label":"Husqvarna"},{"value":"hyosung","label":"Hyosung"},{"value":"indian","label":"Indian"},{"value":"kawasaki","label":"Kawasaki"},{"value":"keeway","label":"Keeway"},{"value":"ktm","label":"KTM"},{"value":"kymco","label":"Kymco"},{"value":"mash","label":"Mash"},{"value":"malaguti","label":"Malaguti"},{"value":"mbk","label":"MBK"},{"value":"moto_guzzi","label":"Moto Guzzi"},{"value":"mv_agusta","label":"MV Agusta"},{"value":"peugeot","label":"Peugeot"},{"value":"piaggio","label":"Piaggio"},{"value":"royal_enfield","label":"Royal Enfield"},{"value":"sherco","label":"Sherco"},{"value":"suzuki","label":"Suzuki"},{"value":"sym","label":"SYM"},{"value":"triumph","label":"Triumph"},{"value":"vespa","label":"Vespa"},{"value":"yamaha","label":"Yamaha"},{"value":"autre","label":"Autre"}]' WHERE StepId = @StepId AND FieldKey = 'brand';

-- Update motoType to dependent on vehicleType
UPDATE StepFields SET FieldType = 'dependent_select' WHERE StepId = @StepId AND FieldKey = 'motoType';

-- Add puissance field after cylindree (order 7)
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @StepId AND FieldKey = 'power')
BEGIN
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepId, 'power', N'Puissance', 'number', N'0', '', '', 'Ch', '', 0, 7, 1, NULL, '');
END

-- Reorder fields
UPDATE StepFields SET DisplayOrder = 8 WHERE StepId = @StepId AND FieldKey = 'mileage';
UPDATE StepFields SET DisplayOrder = 9 WHERE StepId = @StepId AND FieldKey = 'license';
UPDATE StepFields SET DisplayOrder = 10 WHERE StepId = @StepId AND FieldKey = 'color';
UPDATE StepFields SET DisplayOrder = 11 WHERE StepId = @StepId AND FieldKey = 'vehicleState';
UPDATE StepFields SET DisplayOrder = 12 WHERE StepId = @StepId AND FieldKey = 'equipment';

-- Add historique et entretien
IF NOT EXISTS (SELECT 1 FROM StepFields WHERE StepId = @StepId AND FieldKey = 'history')
BEGIN
    INSERT INTO StepFields (StepId, FieldKey, Label, FieldType, Placeholder, Options, DefaultValue, Suffix, HelperText, IsRequired, DisplayOrder, IsActive, MaxLength, ValidationRegex)
    VALUES (@StepId, 'history', N'Historique et entretien', 'multiselect', N'Selectionnez',
    N'[{"value":"carnet_jour","label":"Carnet d''entretien a jour"},{"value":"factures","label":"Factures disponibles"},{"value":"premiere_main","label":"Premiere main"},{"value":"non_fumeur","label":"Non fumeur"},{"value":"concession","label":"Toujours entretenue en concession"},{"value":"revisions","label":"Revisions regulieres"},{"value":"aucun_frais","label":"Aucun frais a prevoir"},{"value":"accident","label":"Vehicule accidente / repare"},{"value":"historique_complet","label":"Historique complet disponible"}]',
    '', '', '', 0, 13, 1, NULL, '');
END

PRINT 'Moto workflow details step updated.';
