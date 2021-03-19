using HelpDesk.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Web.Data
{
    public class HelpDeskDbContext : DbContext
    {
        public HelpDeskDbContext(DbContextOptions<HelpDeskDbContext> options)
            :base(options)
        {
        }
        
        public DbSet<Ticket> Tickets { get; set; }
    }
}