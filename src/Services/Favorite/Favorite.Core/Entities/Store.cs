using System;

namespace Favorite.Core.Entities
{
    public class Store
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string SlugKey { get; set; }
        public Guid OrganizationId { get; set; }
    }
}