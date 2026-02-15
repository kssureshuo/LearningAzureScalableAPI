using LearningAzureScalableAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningAzureScalableAPI.Data
{
    public class AppDbContextFile : DbContext
    {
        public AppDbContextFile(DbContextOptions<AppDbContextFile> options): base(options)
        {                
        }
        public DbSet<Orders> orders { get; set; }
    }
}
