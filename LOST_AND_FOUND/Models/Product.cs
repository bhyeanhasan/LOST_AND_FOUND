using Microsoft.AspNetCore.Identity;

namespace LOST_AND_FOUND.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string? Posted_by { get; set; }

        public DateTime? postedAt { get; set; }

    }
}
