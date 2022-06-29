using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOST_AND_FOUND.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string? Posted_by { get; set; }
        public DateTime? postedAt { get; set; }
        public string? PictureName { get; set; }
        [NotMapped]
        public IFormFile ProductPicture { get; set; }

    }
}
