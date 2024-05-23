using DocumentManagement.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<CaseTransaction> CaseTransactions { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>()
                .HasOne<CaseTransaction>()
                .WithMany()
                .HasForeignKey(d => d.CaseTransactionID);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
