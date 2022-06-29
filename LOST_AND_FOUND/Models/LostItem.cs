namespace LOST_AND_FOUND.Models
{
    public class LostItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PostTime { get; set; }
        public string LostBy { get; set; }
    }
}
