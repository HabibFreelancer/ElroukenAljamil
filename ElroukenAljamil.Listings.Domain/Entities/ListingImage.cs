using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Listings.Domain.Entities
{
    public class ListingImage
    {
        public Guid Id { get; private set; }
        public Guid ListingId { get; private set; }
        public string Url { get; private set; } = default!;
        public int DisplayOrder { get; private set; }


        private ListingImage() { }


        public ListingImage(Guid id, Guid listingId, string url, int displayOrder)
        {
            Id = id;
            ListingId = listingId;
            Url = url;
            DisplayOrder = displayOrder;
        }
    }

}
