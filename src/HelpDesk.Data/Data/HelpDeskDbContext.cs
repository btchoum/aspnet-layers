using System;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Ticket>()
                .Property(e => e.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (TicketStatus)Enum.Parse(typeof(TicketStatus), v));
        }
    }
}