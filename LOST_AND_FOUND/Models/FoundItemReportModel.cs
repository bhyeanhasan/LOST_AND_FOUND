using System.ComponentModel.DataAnnotations.Schema;

namespace LOST_AND_FOUND.Models
{
    public class FoundItemReportModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? PostTime { get; set; }
        public string? FoundBy { get; set; }
        public byte[] PictureName { get; set; }
       
    }
}
