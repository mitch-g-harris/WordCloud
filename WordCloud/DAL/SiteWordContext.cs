using Microsoft.EntityFrameworkCore;
using WordCloud.Models;

namespace WordCloud
{
    public class SiteWordContext : DbContext
    {
        public DbSet<SiteWord> SiteWords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./WordCloud.sqlite");
        }
    }

}
