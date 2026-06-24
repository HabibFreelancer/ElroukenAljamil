using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace ElroukenAljamil.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AiController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpPost("generate-description")]
    public async Task<ActionResult> GenerateDescription([FromBody] JsonElement context)
    {
        var prompt = BuildPrompt(context);

        // Try Hugging Face Inference API (free)
        var hfToken = _configuration["HuggingFace:ApiToken"] ?? "";

        if (!string.IsNullOrEmpty(hfToken))
        {
            try
            {
                var description = await CallHuggingFace(prompt, hfToken);
                if (!string.IsNullOrWhiteSpace(description))
                    return Ok(new { description });
            }
            catch { /* Fallback below */ }
        }

        // Fallback: generate a template-based description
        var category = context.TryGetProperty("category", out var cat) ? cat.GetString() ?? "" : "";
        var propertyType = context.TryGetProperty("propertyType", out var pt) ? pt.GetString() ?? "" : "";
        var isImmobilier    = category.ToLower().Contains("immobilier") || category.ToLower().Contains("immobili");
        var isBureauCommerce = category.ToLower().Contains("bureau") || category.ToLower().Contains("commerce");

        var fallback = isBureauCommerce
            ? GenerateFallbackBureauCommerce(context)
            : isImmobilier
                ? GenerateFallbackImmobilier(context, propertyType)
                : GenerateFallbackDescription(context);

        return Ok(new { description = fallback });
    }

    private string BuildPrompt(JsonElement context)
    {
        var sb = new StringBuilder();
        var category = GetValue(context, "category") ?? "";
        var propertyType = GetValue(context, "propertyType") ?? "";

        // ── Immobilier prompts ──────────────────────────────────────────────
        var isImmobilier = category.ToLower().Contains("immobilier") || category.ToLower().Contains("immobili");
        var isBureauCommerce = category.ToLower().Contains("bureau") || category.ToLower().Contains("commerce");

        if (isBureauCommerce)
        {
            return BuildBureauCommercePrompt(context);
        }

        if (isImmobilier)
        {
            return BuildImmobilierPrompt(context, propertyType);
        }

        var isMoto = category.ToLower().Contains("moto") || (GetValue(context, "cylindree") != null) || (GetValue(context, "motoType") != null);
        var isCaravan = category.ToLower().Contains("caravan");
        var isUtilitaire = category.ToLower().Contains("utilitaire");
        var isNautisme = category.ToLower().Contains("nautis");
        var isEquipement = category.ToLower().Contains("quipement");

        if (isMoto)
        {
            sb.AppendLine("G\u00e9n\u00e8re une description d'annonce de vente de moto en fran\u00e7ais.");
            sb.AppendLine("Voici un exemple du format attendu :");
            sb.AppendLine("");
            sb.AppendLine("Je vends ma Yamaha MT-07 de 2022, une roadster agile et puissante avec seulement 5 000 km au compteur.");
            sb.AppendLine("- Marque : Yamaha");
            sb.AppendLine("- Mod\u00e8le : MT-07");
            sb.AppendLine("- Ann\u00e9e : 2022");
            sb.AppendLine("- Kilom\u00e9trage : 5 000 km");
            sb.AppendLine("- Cylindr\u00e9e : 600 - 900 cm\u00b3");
            sb.AppendLine("- Type : Roadster");
            sb.AppendLine("- Couleur : Noir");
            sb.AppendLine("- \u00c9quipements : ABS, D\u00e9marreur \u00e9lectrique, Carnet d'entretien");
            sb.AppendLine("N'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations ou pour convenir d'un essai !");
        }
        else if (isCaravan)
        {
            sb.AppendLine("G\u00e9n\u00e8re une description d'annonce de vente de camping-car ou caravane en fran\u00e7ais.");
            sb.AppendLine("Voici un exemple du format attendu :");
            sb.AppendLine("");
            sb.AppendLine("Je vends mon camping-car Chausson Flash de 2020, un profil\u00e9 spacieux et bien \u00e9quip\u00e9 avec 25 000 km au compteur.");
            sb.AppendLine("- Type : Camping-car semi int\u00e9gr\u00e9");
            sb.AppendLine("- Mod\u00e8le : Chausson Flash");
            sb.AppendLine("- Ann\u00e9e : 2020");
            sb.AppendLine("- Kilom\u00e9trage : 25 000 km");
            sb.AppendLine("- Couchages : 4 personnes");
            sb.AppendLine("- \u00c9tat : Tr\u00e8s bon \u00e9tat");
            sb.AppendLine("Id\u00e9al pour les vacances en famille. V\u00e9hicule entretenu r\u00e9guli\u00e8rement.");
            sb.AppendLine("N'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations ou pour organiser une visite !");
        }
        else if (isUtilitaire)
        {
            sb.AppendLine("G\u00e9n\u00e8re une description d'annonce de vente de v\u00e9hicule utilitaire en fran\u00e7ais.");
            sb.AppendLine("Voici un exemple du format attendu :");
            sb.AppendLine("");
            sb.AppendLine("Je vends mon Renault Master L2H2 de 2021, un fourgon fiable et spacieux avec seulement 45 000 km.");
            sb.AppendLine("- Marque : Renault");
            sb.AppendLine("- Mod\u00e8le : Master");
            sb.AppendLine("- Ann\u00e9e : 2021");
            sb.AppendLine("- Kilom\u00e9trage : 45 000 km");
            sb.AppendLine("- Carburant : Diesel");
            sb.AppendLine("- Version : L2H2");
            sb.AppendLine("- Volume : 10 m\u00b3");
            sb.AppendLine("- PTAC : 3,5 t");
            sb.AppendLine("- TVA r\u00e9cup\u00e9rable : Oui");
            sb.AppendLine("V\u00e9hicule id\u00e9al pour professionnels. Entretien suivi en concession.");
            sb.AppendLine("N'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations !");
        }
        else if (isNautisme)
        {
            sb.AppendLine("G\u00e9n\u00e8re une description d'annonce de vente de bateau ou v\u00e9hicule nautique en fran\u00e7ais.");
            sb.AppendLine("Voici un exemple du format attendu :");
            sb.AppendLine("");
            sb.AppendLine("Je vends mon Jet Ski Yamaha VX de 2022, en excellent \u00e9tat avec seulement 50 heures de navigation.");
            sb.AppendLine("- Type : Jet Ski");
            sb.AppendLine("- Marque : Yamaha");
            sb.AppendLine("- Mod\u00e8le : VX");
            sb.AppendLine("- Ann\u00e9e : 2022");
            sb.AppendLine("- Heures : 50h");
            sb.AppendLine("- \u00c9tat : Comme neuf");
            sb.AppendLine("Remorque incluse. Id\u00e9al pour la saison estivale.");
            sb.AppendLine("N'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations !");
        }
        else if (isEquipement)
        {
            sb.AppendLine("G\u00e9n\u00e8re une description d'annonce de vente d'\u00e9quipement ou pi\u00e8ce d\u00e9tach\u00e9e en fran\u00e7ais.");
            sb.AppendLine("Voici un exemple du format attendu :");
            sb.AppendLine("");
            sb.AppendLine("Je vends un jeu de 4 jantes aluminium 17 pouces pour BMW S\u00e9rie 3, en tr\u00e8s bon \u00e9tat.");
            sb.AppendLine("- Type : Pneus & jantes");
            sb.AppendLine("- Compatibilit\u00e9 : BMW S\u00e9rie 3 (E90/F30)");
            sb.AppendLine("- Dimensions : 17 pouces");
            sb.AppendLine("- \u00c9tat : Tr\u00e8s bon \u00e9tat, pas de voile");
            sb.AppendLine("- Pneus inclus : Oui (Michelin Pilot Sport 225/45 R17)");
            sb.AppendLine("N'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations !");
        }
        else
        {
            sb.AppendLine("G\u00e9n\u00e8re une description d'annonce de vente de v\u00e9hicule en fran\u00e7ais.");
            sb.AppendLine("Voici un exemple du format attendu :");
            sb.AppendLine("");
            sb.AppendLine("Je vends mon Citro\u00ebn C5 Aircross de 2022, un SUV spacieux et confortable avec seulement 4 564 km au compteur.");
            sb.AppendLine("- Marque : Citro\u00ebn");
            sb.AppendLine("- Mod\u00e8le : C5 Aircross");
            sb.AppendLine("- Ann\u00e9e : 2022");
            sb.AppendLine("- Kilom\u00e9trage : 4 564 km");
            sb.AppendLine("- Motorisation : 130 Ch");
            sb.AppendLine("- Carburant : Essence");
            sb.AppendLine("- Bo\u00eete de vitesses : Automatique");
            sb.AppendLine("- Couleur : Rose");
            sb.AppendLine("- Type de v\u00e9hicule : SUV");
            sb.AppendLine("- Nombre de si\u00e8ges : 5");
            sb.AppendLine("- Puissance fiscale : 7 Cv");
            sb.AppendLine("- Contr\u00f4le technique : Valide jusqu'en 03/2027");
            sb.AppendLine("N'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations ou pour convenir d'un essai !");
        }

        sb.AppendLine("");
        sb.AppendLine("Maintenant g\u00e9n\u00e8re une description EXACTEMENT dans ce format avec les informations suivantes :");
        
        if (context.TryGetProperty("brand", out var brand)) sb.AppendLine($"- Marque: {brand}");
        if (context.TryGetProperty("model", out var model)) sb.AppendLine($"- Mod\u00e8le: {model}");
        if (context.TryGetProperty("year", out var year)) sb.AppendLine($"- Ann\u00e9e: {year}");
        if (context.TryGetProperty("fuel", out var fuel)) sb.AppendLine($"- Carburant: {fuel}");
        if (context.TryGetProperty("gearbox", out var gearbox)) sb.AppendLine($"- Bo\u00eete: {gearbox}");
        if (context.TryGetProperty("mileage", out var mileage)) sb.AppendLine($"- Kilom\u00e9trage: {mileage} km");
        if (context.TryGetProperty("cylindree", out var cyl)) sb.AppendLine($"- Cylindr\u00e9e: {cyl}");
        if (context.TryGetProperty("motoType", out var mt)) sb.AppendLine($"- Type de moto: {mt}");
        if (context.TryGetProperty("vehicleType", out var vt)) sb.AppendLine($"- Type: {vt}");
        if (context.TryGetProperty("fiscalPower", out var fp)) sb.AppendLine($"- Puissance fiscale: {fp} CV");
        if (context.TryGetProperty("dinPower", out var dp)) sb.AppendLine($"- Motorisation: {dp} Ch");
        if (context.TryGetProperty("seats", out var seats)) sb.AppendLine($"- Si\u00e8ges: {seats}");
        if (context.TryGetProperty("doors", out var doors)) sb.AppendLine($"- Portes: {doors}");
        if (context.TryGetProperty("color", out var color)) sb.AppendLine($"- Couleur: {color}");
        if (context.TryGetProperty("technicalControl", out var tc)) sb.AppendLine($"- Contr\u00f4le technique valide jusqu'en: {tc}");
        if (context.TryGetProperty("upholstery", out var uph)) sb.AppendLine($"- Sellerie: {uph}");
        if (context.TryGetProperty("equipment", out var equip)) sb.AppendLine($"- \u00c9quipements: {equip}");
        if (context.TryGetProperty("equipmentType", out var eqType)) sb.AppendLine($"- Type d'\u00e9quipement: {eqType}");
        if (context.TryGetProperty("history", out var hist)) sb.AppendLine($"- Historique: {hist}");
        if (context.TryGetProperty("license", out var lic)) sb.AppendLine($"- Permis: {lic}");
        if (context.TryGetProperty("volume", out var vol)) sb.AppendLine($"- Volume: {vol}");
        if (context.TryGetProperty("ptac", out var ptac)) sb.AppendLine($"- PTAC: {ptac}");
        if (context.TryGetProperty("transmission", out var trans)) sb.AppendLine($"- Transmission: {trans}");
        if (context.TryGetProperty("tvaRecuperable", out var tva)) sb.AppendLine($"- TVA r\u00e9cup\u00e9rable: {tva}");
        if (context.TryGetProperty("title", out var title)) sb.AppendLine($"- Titre: {title}");

        sb.AppendLine("\nR\u00e9ponds UNIQUEMENT avec la description g\u00e9n\u00e9r\u00e9e, sans commentaire ni explication.");
        return sb.ToString();
    }

    private string BuildImmobilierPrompt(JsonElement context, string propertyType)
    {
        var sb = new StringBuilder();

        // ── Choose template by property type ──
        switch (propertyType.ToLower())
        {
            case "maison":
                sb.AppendLine("Génère une description d'annonce immobilière pour une MAISON à vendre, en français, dans un style professionnel et attractif.");
                sb.AppendLine("Voici un exemple du format attendu :");
                sb.AppendLine();
                sb.AppendLine("Magnifique maison de ville de 120 m² proposée à la vente, idéalement située à Tunis.");
                sb.AppendLine("Elle se compose de 5 pièces dont 3 chambres spacieuses, 2 salles de bain et une cuisine équipée ouverte sur le salon.");
                sb.AppendLine("- Surface habitable : 120 m²");
                sb.AppendLine("- Nombre de pièces : 5");
                sb.AppendLine("- Chambres : 3");
                sb.AppendLine("- Salles de bain : 2");
                sb.AppendLine("- Cuisine : Équipée, Ouverte");
                sb.AppendLine("- Niveaux : 2");
                sb.AppendLine("- État : Très bon état");
                sb.AppendLine("- Année de construction : 2010");
                sb.AppendLine("- Chauffage : Gaz");
                sb.AppendLine("- Extérieur : Jardin, Terrasse");
                sb.AppendLine("Bien rare sur le marché, à visiter sans tarder !");
                sb.AppendLine("N'hésitez pas à me contacter pour organiser une visite.");
                break;

            case "appartement":
                sb.AppendLine("Génère une description d'annonce immobilière pour un APPARTEMENT à vendre, en français, dans un style professionnel et attractif.");
                sb.AppendLine("Voici un exemple du format attendu :");
                sb.AppendLine();
                sb.AppendLine("Bel appartement lumineux de 75 m² en plein centre-ville, au 4ème étage avec ascenseur.");
                sb.AppendLine("Il comprend un séjour avec cuisine ouverte, 2 chambres, 1 salle de bain et un balcon avec vue dégagée.");
                sb.AppendLine("- Surface habitable : 75 m²");
                sb.AppendLine("- Nombre de pièces : 3");
                sb.AppendLine("- Chambres : 2");
                sb.AppendLine("- Salles de bain : 1");
                sb.AppendLine("- Cuisine : Ouverte");
                sb.AppendLine("- Étage : 4");
                sb.AppendLine("- Ascenseur : Oui");
                sb.AppendLine("- Nombre d'étages dans l'immeuble : 8");
                sb.AppendLine("- État : Rénové");
                sb.AppendLine("- Année de construction : 2005");
                sb.AppendLine("- Chauffage : Électricité");
                sb.AppendLine("- Extérieur : Balcon");
                sb.AppendLine("- Exposition : Sud");
                sb.AppendLine("Idéal pour une famille ou un investissement locatif.");
                sb.AppendLine("N'hésitez pas à me contacter pour organiser une visite.");
                break;

            case "terrain":
                sb.AppendLine("Génère une description d'annonce immobilière pour un TERRAIN à vendre, en français, dans un style professionnel et attractif.");
                sb.AppendLine("Voici un exemple du format attendu :");
                sb.AppendLine();
                sb.AppendLine("Terrain constructible de 500 m² à vendre dans un quartier résidentiel calme et bien desservi.");
                sb.AppendLine("Idéalement situé à proximité des commodités, ce terrain offre de belles possibilités de construction.");
                sb.AppendLine("- Surface : 500 m²");
                sb.AppendLine("- Nature : Terrain constructible");
                sb.AppendLine("- Surface totale du terrain : 500 m²");
                sb.AppendLine("Toutes les viabilisations sont en place (eau, électricité, assainissement).");
                sb.AppendLine("Opportunité rare à saisir rapidement !");
                sb.AppendLine("N'hésitez pas à me contacter pour plus d'informations ou pour une visite.");
                break;

            case "parking":
                sb.AppendLine("Génère une description d'annonce immobilière pour un PARKING ou GARAGE à vendre, en français, dans un style professionnel et concis.");
                sb.AppendLine("Voici un exemple du format attendu :");
                sb.AppendLine();
                sb.AppendLine("Box fermé sécurisé de 15 m² à vendre dans une résidence gardée en plein centre-ville.");
                sb.AppendLine("Accès 24h/24, idéal pour protéger votre véhicule ou l'utiliser comme espace de stockage.");
                sb.AppendLine("- Nature : Box ou garage fermé");
                sb.AppendLine("- État : Très bon état");
                sb.AppendLine("- Année de construction : 2008");
                sb.AppendLine("Investissement idéal ou usage personnel.");
                sb.AppendLine("N'hésitez pas à me contacter pour plus d'informations.");
                break;

            default: // "autre" et fallback
                sb.AppendLine("Génère une description d'annonce immobilière en français, dans un style professionnel et attractif.");
                sb.AppendLine("Voici un exemple du format attendu :");
                sb.AppendLine();
                sb.AppendLine("Bien immobilier de 90 m² à vendre, en très bon état général, situé dans un quartier recherché.");
                sb.AppendLine("Il se compose de 4 pièces lumineuses avec de belles prestations.");
                sb.AppendLine("- Surface : 90 m²");
                sb.AppendLine("- Pièces : 4");
                sb.AppendLine("- État : Très bon état");
                sb.AppendLine("Disponible rapidement. N'hésitez pas à me contacter pour organiser une visite.");
                break;
        }

        // ── Inject actual field values ──
        sb.AppendLine();
        sb.AppendLine("Maintenant génère une description EXACTEMENT dans ce format avec les informations suivantes :");

        var surface        = GetValue(context, "surface");
        var rooms          = GetValue(context, "rooms");
        var bedrooms       = GetValue(context, "bedrooms");
        var bathrooms      = GetValue(context, "bathrooms");
        var cuisine        = GetValue(context, "cuisine");
        var levels         = GetValue(context, "levels");
        var floor          = GetValue(context, "floor");
        var totalFloors    = GetValue(context, "totalFloors");
        var elevator       = GetValue(context, "elevator");
        var constructYear  = GetValue(context, "constructionYear");
        var conditionVal   = GetValue(context, "condition");
        var propertyNature = GetValue(context, "propertyNature");
        var terrainNature  = GetValue(context, "terrainNature");
        var parkingNature  = GetValue(context, "parkingNature");
        var features       = GetValue(context, "features");
        var landSurface    = GetValue(context, "landSurface");
        var parkingSpots   = GetValue(context, "parking");
        var heatingMode    = GetValue(context, "heatingMode");
        var exterior       = GetValue(context, "exterior");
        var exposure       = GetValue(context, "exposure");
        var address        = GetValue(context, "address");
        var title          = GetValue(context, "title");

        if (!string.IsNullOrEmpty(title))        sb.AppendLine($"- Titre souhaité : {title}");
        if (!string.IsNullOrEmpty(address))      sb.AppendLine($"- Localisation : {address}");
        if (!string.IsNullOrEmpty(surface))      sb.AppendLine($"- Surface habitable : {surface} m²");
        if (!string.IsNullOrEmpty(landSurface))  sb.AppendLine($"- Surface totale du terrain : {landSurface} m²");
        if (!string.IsNullOrEmpty(rooms))        sb.AppendLine($"- Nombre de pièces : {rooms}");
        if (!string.IsNullOrEmpty(bedrooms))     sb.AppendLine($"- Chambres : {bedrooms}");
        if (!string.IsNullOrEmpty(bathrooms))    sb.AppendLine($"- Salles de bain : {bathrooms}");
        if (!string.IsNullOrEmpty(cuisine))      sb.AppendLine($"- Cuisine : {cuisine}");
        if (!string.IsNullOrEmpty(levels))       sb.AppendLine($"- Niveaux : {levels}");
        if (!string.IsNullOrEmpty(floor))        sb.AppendLine($"- Étage : {floor}");
        if (!string.IsNullOrEmpty(totalFloors))  sb.AppendLine($"- Nombre d'étages dans l'immeuble : {totalFloors}");
        if (!string.IsNullOrEmpty(elevator))     sb.AppendLine($"- Ascenseur : {(elevator == "true" ? "Oui" : "Non")}");
        if (!string.IsNullOrEmpty(conditionVal)) sb.AppendLine($"- État du bien : {conditionVal}");
        if (!string.IsNullOrEmpty(constructYear))sb.AppendLine($"- Année de construction : {constructYear}");
        if (!string.IsNullOrEmpty(propertyNature))sb.AppendLine($"- Nature du bien : {propertyNature}");
        if (!string.IsNullOrEmpty(terrainNature)) sb.AppendLine($"- Nature du terrain : {terrainNature}");
        if (!string.IsNullOrEmpty(parkingNature)) sb.AppendLine($"- Nature du parking : {parkingNature}");
        if (!string.IsNullOrEmpty(features))     sb.AppendLine($"- Caractéristiques : {features}");
        if (!string.IsNullOrEmpty(parkingSpots)) sb.AppendLine($"- Places de parking : {parkingSpots}");
        if (!string.IsNullOrEmpty(heatingMode))  sb.AppendLine($"- Chauffage : {heatingMode}");
        if (!string.IsNullOrEmpty(exterior))     sb.AppendLine($"- Extérieur : {exterior}");
        if (!string.IsNullOrEmpty(exposure))     sb.AppendLine($"- Exposition : {exposure}");

        sb.AppendLine();
        sb.AppendLine("Réponds UNIQUEMENT avec la description générée, sans commentaire ni explication.");
        return sb.ToString();
    }

    private string GenerateFallbackImmobilier(JsonElement context, string propertyType)
    {
        var sb = new StringBuilder();

        var surface       = GetValue(context, "surface") ?? "";
        var rooms         = GetValue(context, "rooms") ?? "";
        var bedrooms      = GetValue(context, "bedrooms") ?? "";
        var bathrooms     = GetValue(context, "bathrooms") ?? "";
        var cuisine       = GetValue(context, "cuisine") ?? "";
        var levels        = GetValue(context, "levels") ?? "";
        var floor         = GetValue(context, "floor") ?? "";
        var totalFloors   = GetValue(context, "totalFloors") ?? "";
        var elevator      = GetValue(context, "elevator") ?? "";
        var constructYear = GetValue(context, "constructionYear") ?? "";
        var condition     = GetValue(context, "condition") ?? "";
        var propNature    = GetValue(context, "propertyNature") ?? GetValue(context, "terrainNature") ?? GetValue(context, "parkingNature") ?? "";
        var features      = GetValue(context, "features") ?? "";
        var landSurface   = GetValue(context, "landSurface") ?? "";
        var parking       = GetValue(context, "parking") ?? "";
        var heatingMode   = GetValue(context, "heatingMode") ?? "";
        var exterior      = GetValue(context, "exterior") ?? "";
        var exposure      = GetValue(context, "exposure") ?? "";
        var address       = GetValue(context, "address") ?? "";
        var title         = GetValue(context, "title") ?? "";

        // Intro sentence by type
        switch (propertyType.ToLower())
        {
            case "maison":
                sb.Append(!string.IsNullOrEmpty(surface)
                    ? $"Belle maison de {surface} m² à vendre"
                    : "Belle maison à vendre");
                if (!string.IsNullOrEmpty(address)) sb.Append($", idéalement située à {address}");
                sb.AppendLine(".");
                if (!string.IsNullOrEmpty(rooms))    sb.AppendLine($"Elle se compose de {rooms} pièces" + (!string.IsNullOrEmpty(bedrooms) ? $" dont {bedrooms} chambre(s)" : "") + ".");
                break;

            case "appartement":
                sb.Append(!string.IsNullOrEmpty(surface)
                    ? $"Bel appartement de {surface} m² à vendre"
                    : "Bel appartement à vendre");
                if (!string.IsNullOrEmpty(floor)) sb.Append($", au {floor}ème étage");
                if (!string.IsNullOrEmpty(address)) sb.Append($" à {address}");
                sb.AppendLine(".");
                if (!string.IsNullOrEmpty(rooms)) sb.AppendLine($"Il comprend {rooms} pièces" + (!string.IsNullOrEmpty(bedrooms) ? $" dont {bedrooms} chambre(s)" : "") + ".");
                break;

            case "terrain":
                sb.Append(!string.IsNullOrEmpty(surface)
                    ? $"Terrain de {surface} m² à vendre"
                    : "Terrain à vendre");
                if (!string.IsNullOrEmpty(address)) sb.Append($" à {address}");
                sb.AppendLine(".");
                if (!string.IsNullOrEmpty(propNature)) sb.AppendLine($"Nature : {propNature}.");
                break;

            case "parking":
                sb.Append("Emplacement de parking à vendre");
                if (!string.IsNullOrEmpty(address)) sb.Append($" à {address}");
                sb.AppendLine(".");
                if (!string.IsNullOrEmpty(propNature)) sb.AppendLine($"Nature : {propNature}.");
                break;

            default:
                sb.Append(!string.IsNullOrEmpty(surface)
                    ? $"Bien immobilier de {surface} m² à vendre"
                    : "Bien immobilier à vendre");
                if (!string.IsNullOrEmpty(address)) sb.Append($" à {address}");
                sb.AppendLine(".");
                break;
        }

        // Details block
        if (!string.IsNullOrEmpty(surface))       sb.AppendLine($"- Surface habitable : {surface} m²");
        if (!string.IsNullOrEmpty(landSurface))   sb.AppendLine($"- Surface totale du terrain : {landSurface} m²");
        if (!string.IsNullOrEmpty(rooms))         sb.AppendLine($"- Nombre de pièces : {rooms}");
        if (!string.IsNullOrEmpty(bedrooms))      sb.AppendLine($"- Chambres : {bedrooms}");
        if (!string.IsNullOrEmpty(bathrooms))     sb.AppendLine($"- Salles de bain : {bathrooms}");
        if (!string.IsNullOrEmpty(cuisine))       sb.AppendLine($"- Cuisine : {cuisine}");
        if (!string.IsNullOrEmpty(levels))        sb.AppendLine($"- Niveaux : {levels}");
        if (!string.IsNullOrEmpty(floor))         sb.AppendLine($"- Étage : {floor}");
        if (!string.IsNullOrEmpty(totalFloors))   sb.AppendLine($"- Nombre d'étages dans l'immeuble : {totalFloors}");
        if (!string.IsNullOrEmpty(elevator))      sb.AppendLine($"- Ascenseur : {(elevator == "true" ? "Oui" : "Non")}");
        if (!string.IsNullOrEmpty(condition))     sb.AppendLine($"- État : {condition}");
        if (!string.IsNullOrEmpty(constructYear)) sb.AppendLine($"- Année de construction : {constructYear}");
        if (!string.IsNullOrEmpty(propNature))    sb.AppendLine($"- Nature : {propNature}");
        if (!string.IsNullOrEmpty(features))      sb.AppendLine($"- Caractéristiques : {features}");
        if (!string.IsNullOrEmpty(parking))       sb.AppendLine($"- Places de parking : {parking}");
        if (!string.IsNullOrEmpty(heatingMode))   sb.AppendLine($"- Chauffage : {heatingMode}");
        if (!string.IsNullOrEmpty(exterior))      sb.AppendLine($"- Extérieur : {exterior}");
        if (!string.IsNullOrEmpty(exposure))      sb.AppendLine($"- Exposition : {exposure}");

        sb.AppendLine();
        sb.AppendLine("N'hésitez pas à me contacter pour organiser une visite !");

        return sb.ToString().Trim();
    }

    private string BuildBureauCommercePrompt(JsonElement context)
    {
        var sb = new StringBuilder();
        var businessType  = GetValue(context, "businessType") ?? "";
        var surface       = GetValue(context, "surface") ?? "";
        var divSurface    = GetValue(context, "divisibleSurface") ?? "";
        var levels        = GetValue(context, "levels") ?? "";
        var floor         = GetValue(context, "floor") ?? "";
        var elevator      = GetValue(context, "elevator") ?? "";
        var exterior      = GetValue(context, "exterior") ?? "";
        var parking       = GetValue(context, "parking") ?? "";
        var constructYear = GetValue(context, "constructionYear") ?? "";
        var availableFrom = GetValue(context, "availableFrom") ?? "";
        var salePrice     = GetValue(context, "salePrice") ?? "";
        var taxFonciere   = GetValue(context, "taxFonciere") ?? "";
        var chargesCopro  = GetValue(context, "chargesCopro") ?? "";
        var address       = GetValue(context, "address") ?? "";
        var title         = GetValue(context, "title") ?? "";

        // Choose intro example by businessType
        var typeLabel = businessType switch
        {
            "bureaux"            => "bureau",
            "conteneurs"         => "conteneur aménagé",
            "entrepots"          => "entrepôt",
            "restaurants_hotels" => "restaurant / hôtel",
            "boutiques_kiosques" => "boutique / kiosque",
            _                    => "local commercial"
        };

        sb.AppendLine($"Génère une description d'annonce immobilière professionnelle pour un LOCAL DE TYPE \"{typeLabel.ToUpper()}\" à vendre, en français.");
        sb.AppendLine("Le style doit être professionnel, précis et attractif pour des acheteurs professionnels.");
        sb.AppendLine("Voici un exemple du format attendu :");
        sb.AppendLine();
        sb.AppendLine($"Excellent {typeLabel} de 120 m² à vendre en plein cœur de Tunis, idéalement situé dans une zone à fort passage.");
        sb.AppendLine("Le bien se présente au 2ème étage d'un immeuble récent avec ascenseur, offrant un espace lumineux et modulable.");
        sb.AppendLine("- Type d'activité : Bureaux");
        sb.AppendLine("- Surface habitable : 120 m²");
        sb.AppendLine("- Surface divisible minimale : 40 m²");
        sb.AppendLine("- Nombre d'étages : 1");
        sb.AppendLine("- Étage : 2");
        sb.AppendLine("- Ascenseur : Oui");
        sb.AppendLine("- Extérieur : Terrasse");
        sb.AppendLine("- Places de parking : 2");
        sb.AppendLine("- Année de construction : 2015");
        sb.AppendLine("Opportunité rare pour investisseur ou profession libérale. Disponible immédiatement.");
        sb.AppendLine("N'hésitez pas à nous contacter pour organiser une visite.");
        sb.AppendLine();
        sb.AppendLine("Maintenant génère une description EXACTEMENT dans ce format avec les informations suivantes :");

        if (!string.IsNullOrEmpty(title))        sb.AppendLine($"- Titre souhaité : {title}");
        if (!string.IsNullOrEmpty(address))      sb.AppendLine($"- Localisation : {address}");
        if (!string.IsNullOrEmpty(businessType)) sb.AppendLine($"- Type d'activité : {typeLabel}");
        if (!string.IsNullOrEmpty(surface))      sb.AppendLine($"- Surface habitable : {surface} m²");
        if (!string.IsNullOrEmpty(divSurface))   sb.AppendLine($"- Surface divisible minimale : {divSurface} m²");
        if (!string.IsNullOrEmpty(levels))       sb.AppendLine($"- Nombre d'étages : {levels}");
        if (!string.IsNullOrEmpty(floor))        sb.AppendLine($"- Étage : {floor}");
        if (!string.IsNullOrEmpty(elevator))     sb.AppendLine($"- Ascenseur : {(elevator == "true" ? "Oui" : "Non")}");
        if (!string.IsNullOrEmpty(exterior))     sb.AppendLine($"- Extérieur : {exterior}");
        if (!string.IsNullOrEmpty(parking))      sb.AppendLine($"- Places de parking : {parking}");
        if (!string.IsNullOrEmpty(constructYear))sb.AppendLine($"- Année de construction : {constructYear}");
        if (!string.IsNullOrEmpty(availableFrom))sb.AppendLine($"- Disponible à partir de : {availableFrom}");
        if (!string.IsNullOrEmpty(salePrice))    sb.AppendLine($"- Prix de vente : {salePrice} TND");
        if (!string.IsNullOrEmpty(taxFonciere))  sb.AppendLine($"- Taxe foncière : {taxFonciere} TND/an");
        if (!string.IsNullOrEmpty(chargesCopro)) sb.AppendLine($"- Charges de copropriété : {chargesCopro} TND/an");

        sb.AppendLine();
        sb.AppendLine("Réponds UNIQUEMENT avec la description générée, sans commentaire ni explication.");
        return sb.ToString();
    }

    private string GenerateFallbackBureauCommerce(JsonElement context)
    {
        var sb = new StringBuilder();
        var businessType  = GetValue(context, "businessType") ?? "";
        var surface       = GetValue(context, "surface") ?? "";
        var divSurface    = GetValue(context, "divisibleSurface") ?? "";
        var levels        = GetValue(context, "levels") ?? "";
        var floor         = GetValue(context, "floor") ?? "";
        var elevator      = GetValue(context, "elevator") ?? "";
        var exterior      = GetValue(context, "exterior") ?? "";
        var parking       = GetValue(context, "parking") ?? "";
        var constructYear = GetValue(context, "constructionYear") ?? "";
        var availableFrom = GetValue(context, "availableFrom") ?? "";
        var salePrice     = GetValue(context, "salePrice") ?? "";
        var taxFonciere   = GetValue(context, "taxFonciere") ?? "";
        var chargesCopro  = GetValue(context, "chargesCopro") ?? "";
        var address       = GetValue(context, "address") ?? "";

        var typeLabel = businessType switch
        {
            "bureaux"            => "bureau",
            "conteneurs"         => "conteneur aménagé",
            "entrepots"          => "entrepôt",
            "restaurants_hotels" => "local restauration/hôtellerie",
            "boutiques_kiosques" => "boutique/kiosque",
            _                    => "local commercial"
        };

        sb.Append(!string.IsNullOrEmpty(surface)
            ? $"{char.ToUpper(typeLabel[0])}{typeLabel.Substring(1)} de {surface} m² à vendre"
            : $"{char.ToUpper(typeLabel[0])}{typeLabel.Substring(1)} à vendre");
        if (!string.IsNullOrEmpty(address)) sb.Append($" à {address}");
        sb.AppendLine(".");

        if (!string.IsNullOrEmpty(businessType))  sb.AppendLine($"- Type d'activité : {typeLabel}");
        if (!string.IsNullOrEmpty(surface))       sb.AppendLine($"- Surface habitable : {surface} m²");
        if (!string.IsNullOrEmpty(divSurface))    sb.AppendLine($"- Surface divisible minimale : {divSurface} m²");
        if (!string.IsNullOrEmpty(levels))        sb.AppendLine($"- Nombre d'étages : {levels}");
        if (!string.IsNullOrEmpty(floor))         sb.AppendLine($"- Étage : {floor}");
        if (!string.IsNullOrEmpty(elevator))      sb.AppendLine($"- Ascenseur : {(elevator == "true" ? "Oui" : "Non")}");
        if (!string.IsNullOrEmpty(exterior))      sb.AppendLine($"- Extérieur : {exterior}");
        if (!string.IsNullOrEmpty(parking))       sb.AppendLine($"- Places de parking : {parking}");
        if (!string.IsNullOrEmpty(constructYear)) sb.AppendLine($"- Année de construction : {constructYear}");
        if (!string.IsNullOrEmpty(availableFrom)) sb.AppendLine($"- Disponible à partir de : {availableFrom}");
        if (!string.IsNullOrEmpty(salePrice))     sb.AppendLine($"- Prix de vente : {salePrice} TND");
        if (!string.IsNullOrEmpty(taxFonciere))   sb.AppendLine($"- Taxe foncière : {taxFonciere} TND/an");
        if (!string.IsNullOrEmpty(chargesCopro))  sb.AppendLine($"- Charges copropriété : {chargesCopro} TND/an");

        sb.AppendLine();
        sb.AppendLine("N'hésitez pas à nous contacter pour organiser une visite !");
        return sb.ToString().Trim();
    }

    private async Task<string?> CallHuggingFace(string prompt, string token)
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        // Using a free model on Hugging Face
        var url = "https://api-inference.huggingface.co/models/mistralai/Mistral-7B-Instruct-v0.2";

        var payload = new
        {
            inputs = $"<s>[INST] {prompt} [/INST]",
            parameters = new { max_new_tokens = 300, temperature = 0.7 }
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);

            // HF returns array of objects with "generated_text"
            if (result.ValueKind == JsonValueKind.Array && result.GetArrayLength() > 0)
            {
                var generated = result[0].GetProperty("generated_text").GetString() ?? "";
                // Remove the prompt from the response
                var instEnd = generated.LastIndexOf("[/INST]");
                if (instEnd >= 0) generated = generated.Substring(instEnd + 7).Trim();
                return generated;
            }
        }

        return null;
    }

    private string GenerateFallbackDescription(JsonElement context)
    {
        var sb = new StringBuilder();

        var brand = GetValue(context, "brand") ?? "";
        var model = GetValue(context, "model") ?? "";
        var year = GetValue(context, "year") ?? "";
        var fuel = GetValue(context, "fuel") ?? "";
        var mileage = GetValue(context, "mileage") ?? "";
        var gearbox = GetValue(context, "gearbox") ?? "";
        var dinPower = GetValue(context, "dinPower") ?? "";
        var fiscalPower = GetValue(context, "fiscalPower") ?? "";
        var vehicleType = GetValue(context, "vehicleType") ?? "";
        var seats = GetValue(context, "seats") ?? "";
        var doors = GetValue(context, "doors") ?? "";
        var color = GetValue(context, "color") ?? "";
        var technicalControl = GetValue(context, "technicalControl") ?? "";
        var upholstery = GetValue(context, "upholstery") ?? "";
        var equipment = GetValue(context, "equipment") ?? "";
        var history = GetValue(context, "history") ?? "";

        // Intro
        sb.Append($"Je vends mon {brand} {model}");
        if (!string.IsNullOrEmpty(year)) sb.Append($" de {year}");
        if (!string.IsNullOrEmpty(vehicleType)) sb.Append($", un {vehicleType} spacieux et confortable");
        if (!string.IsNullOrEmpty(mileage)) sb.Append($" avec seulement {mileage} km au compteur");
        sb.AppendLine(".");

        // Details
        if (!string.IsNullOrEmpty(brand)) sb.AppendLine($"- Marque : {brand}");
        if (!string.IsNullOrEmpty(model)) sb.AppendLine($"- Mod\u00e8le : {model}");
        if (!string.IsNullOrEmpty(year)) sb.AppendLine($"- Ann\u00e9e : {year}");
        if (!string.IsNullOrEmpty(mileage)) sb.AppendLine($"- Kilom\u00e9trage : {mileage} km");
        if (!string.IsNullOrEmpty(dinPower)) sb.AppendLine($"- Motorisation : {dinPower} Ch");
        if (!string.IsNullOrEmpty(fuel)) sb.AppendLine($"- Carburant : {fuel}");
        if (!string.IsNullOrEmpty(gearbox)) sb.AppendLine($"- Bo\u00eete de vitesses : {gearbox}");
        if (!string.IsNullOrEmpty(color)) sb.AppendLine($"- Couleur : {color}");
        if (!string.IsNullOrEmpty(vehicleType)) sb.AppendLine($"- Type de v\u00e9hicule : {vehicleType}");
        if (!string.IsNullOrEmpty(seats)) sb.AppendLine($"- Nombre de si\u00e8ges : {seats}");
        if (!string.IsNullOrEmpty(doors)) sb.AppendLine($"- Nombre de portes : {doors}");
        if (!string.IsNullOrEmpty(fiscalPower)) sb.AppendLine($"- Puissance fiscale : {fiscalPower} CV");
        if (!string.IsNullOrEmpty(technicalControl)) sb.AppendLine($"- Contr\u00f4le technique : Valide jusqu'en {technicalControl}");
        if (!string.IsNullOrEmpty(upholstery)) sb.AppendLine($"- Sellerie : {upholstery}");
        if (!string.IsNullOrEmpty(equipment)) sb.AppendLine($"- \u00c9quipements : {equipment}");
        if (!string.IsNullOrEmpty(history)) sb.AppendLine($"- Historique : {history}");

        sb.AppendLine();
        sb.AppendLine("N'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations ou pour convenir d'un essai !");

        return sb.ToString().Trim();
    }

    private string? GetValue(JsonElement element, string key)
    {
        if (element.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.String)
            return prop.GetString();
        return null;
    }
}
