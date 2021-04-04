namespace ExchangeRates
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class ModelExchangeRate : DbContext
    {
        public ModelExchangeRate()
            : base("name=ModelExchangeRate")
        {
        }

        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<DailyRate> DailyRate { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Currency>()
                .Property(e => e.NumCode)
                .IsUnicode(false);

            modelBuilder.Entity<Currency>()
                .Property(e => e.CharCode)
                .IsUnicode(false);

            modelBuilder.Entity<Currency>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Currency>()
                .HasMany(e => e.DailyRate)
                .WithRequired(e => e.Currency)
                .HasForeignKey(e => e.IdCurrency)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DailyRate>()
                .Property(e => e.IdCurrency)
                .IsUnicode(false);

            modelBuilder.Entity<DailyRate>()
                .Property(e => e.Rate)
                .HasPrecision(10, 4);
        }
    }
}
