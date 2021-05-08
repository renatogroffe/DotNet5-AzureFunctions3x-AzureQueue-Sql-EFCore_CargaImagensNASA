using Microsoft.EntityFrameworkCore;

namespace FunctionAppImagensNASA.Data
{
    public class NASAContext : DbContext
    {
        public DbSet<CargaImagemNASA> CargasImagemNASA { get; set; }

        public NASAContext(DbContextOptions<NASAContext> options) :
            base(options)
        {
        }        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CargaImagemNASA>()
                .HasKey(c => c.Id);
        }
    }
}