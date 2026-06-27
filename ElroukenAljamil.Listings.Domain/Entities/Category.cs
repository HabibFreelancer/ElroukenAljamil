using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Listings.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }
        public Guid? ParentCategoryId { get; private set; }


        private Category() { }


        public static Category Create(string name, string? description = null, Guid? parentId = null)
        {
            return new Category
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                ParentCategoryId = parentId,
                CreatedAt = DateTime.UtcNow
            };
        }
    }

}
