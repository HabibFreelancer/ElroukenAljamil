using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.BuildingBlocks.Common.Results
{

    /// <summary>
    /// Résultat paginé pour les requêtes de liste.
    /// </summary>
    public class PagedResult<T>
    {
        // Utilisation de { get; init; } au lieu de { get; }
        public IReadOnlyList<T> Items { get; init; } = new List<T>();
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;

        public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
    }

}
