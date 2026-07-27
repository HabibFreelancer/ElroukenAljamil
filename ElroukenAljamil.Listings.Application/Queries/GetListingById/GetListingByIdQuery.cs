using System.Text.Json;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetListingById;

public record GetListingByIdQuery(int Id) : IRequest<AnnonceDetailDto?>;

public class GetListingByIdQueryHandler : IRequestHandler<GetListingByIdQuery, AnnonceDetailDto?>
{
    private readonly IAnnonceRepository _repository;
    public GetListingByIdQueryHandler(IAnnonceRepository repository) => _repository = repository;

    public async Task<AnnonceDetailDto?> Handle(GetListingByIdQuery request, CancellationToken ct)
    {
        var (annonce, views, favorites) = await _repository.GetListingByIdAsync(request.Id, ct);
        if (annonce == null) return null;

        var catName  = annonce.Category?.Name?.ToLower() ?? "";
        var menuName = annonce.Category?.Menu?.Name?.ToLower() ?? "";

        var isImmobilier = menuName.Contains("immobilier") || catName.Contains("immobilier")
                        || catName.Contains("immobili")    || catName.Contains("coloc")
                        || catName.Contains("location");

        var isColocation = catName.Contains("coloc");

        Dictionary<string, JsonElement>? extra = null;
        if (!string.IsNullOrEmpty(annonce.ExtraData))
        {
            try { extra = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(annonce.ExtraData); }
            catch { /* keep null */ }
        }

        Dictionary<string, string>? immobilierDetails = null;
        if (isImmobilier && extra != null)
        {
            immobilierDetails = new()
            {
                ["propertyType"]     = GetStr(extra, "propertyType"),
                ["surface"]          = GetStr(extra, "surface"),
                ["rooms"]            = GetStr(extra, "rooms"),
                ["bedrooms"]         = GetStr(extra, "bedrooms"),
                ["bathrooms"]        = GetStr(extra, "bathrooms"),
                ["cuisine"]          = GetStr(extra, "cuisine"),
                ["levels"]           = GetStr(extra, "levels"),
                ["floor"]            = GetStr(extra, "floor"),
                ["totalFloors"]      = GetStr(extra, "totalFloors"),
                ["elevator"]         = GetStr(extra, "elevator"),
                ["constructionYear"] = GetStr(extra, "constructionYear"),
                ["condition"]        = GetStr(extra, "condition"),
                ["propertyNature"]   = GetStr(extra, "propertyNature"),
                ["terrainNature"]    = GetStr(extra, "terrainNature"),
                ["parkingNature"]    = GetStr(extra, "parkingNature"),
                ["features"]         = GetStr(extra, "features"),
                ["landSurface"]      = GetStr(extra, "landSurface"),
                ["parkingSpots"]     = GetStr(extra, "parking"),
                ["heatingMode"]      = GetStr(extra, "heatingMode"),
                ["exterior"]         = GetStr(extra, "exterior"),
                ["exposure"]         = GetStr(extra, "exposure"),
                ["availableFrom"]    = GetStr(extra, "availableFrom"),
                ["furnished"]        = GetStr(extra, "furnished"),
                ["monthlyRent"]      = GetStr(extra, "monthlyRent"),
                ["deposit"]          = GetStr(extra, "deposit"),
                ["roomType"]         = GetStr(extra, "roomType"),
                ["roommatesCount"]   = GetStr(extra, "roommatesCount"),
                ["smokingPolicy"]    = GetStr(extra, "smokingPolicy"),
                ["petsAllowed"]      = GetStr(extra, "petsAllowed"),
                ["heatingType"]      = GetStr(extra, "heatingType"),
            };
        }

        return new AnnonceDetailDto
        {
            Id                 = annonce.Id,
            Title              = annonce.Title,
            Description        = annonce.Description,
            Price              = annonce.Price,
            CategoryId         = annonce.CategoryId,
            AdType             = annonce.AdType,
            Condition          = annonce.Condition,
            Location           = annonce.Location,
            Phone              = annonce.Phone,
            Email              = annonce.Email,
            HidePhone          = annonce.HidePhone,
            ExtraData          = annonce.ExtraData,
            Status             = annonce.Status,
            CreatedAt          = annonce.CreatedAt,
            Category           = annonce.Category?.Name ?? "",
            Menu               = annonce.Category?.Menu?.Name ?? "",
            IsImmobilier       = isImmobilier,
            IsColocation       = isColocation,
            ImmobilierDetails  = immobilierDetails,
            Views              = views,
            Favorites          = favorites
        };
    }

    private static string GetStr(Dictionary<string, JsonElement> extra, string key)
    {
        if (!extra.TryGetValue(key, out var el)) return "";
        return el.ValueKind switch
        {
            JsonValueKind.String => el.GetString() ?? "",
            JsonValueKind.Number => el.GetRawText(),
            JsonValueKind.True   => "true",
            JsonValueKind.False  => "false",
            JsonValueKind.Array  => string.Join(", ", el.EnumerateArray()
                                        .Select(e => e.GetString() ?? e.GetRawText())),
            _                    => el.GetRawText()
        };
    }
}
