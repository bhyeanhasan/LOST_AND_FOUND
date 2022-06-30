using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LOST_AND_FOUND.Models;

namespace LOST_AND_FOUND.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<LOST_AND_FOUND.Models.FoundItem>? FoundItem { get; set; }
        public DbSet<LOST_AND_FOUND.Models.LostItem>? LostItem { get; set; }
    }
}