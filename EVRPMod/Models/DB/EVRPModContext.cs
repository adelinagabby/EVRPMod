using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EVRPMod.Models.DB
{
    public partial class EVRPModContext : DbContext
    {
        public EVRPModContext()
        {
        }

        public EVRPModContext(DbContextOptions<EVRPModContext> options)
            : base(options)
        {
        }

        public virtual DbSet<customerData> customerData { get; set; }
        public virtual DbSet<depotData> depotData { get; set; }
        public virtual DbSet<kitType> kitType { get; set; }
        public virtual DbSet<vehicleData> vehicleData { get; set; }
        public virtual DbSet<vehicleInDepot> vehicleInDepot { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-IDS9RKI;Initial Catalog=EVRPMod;Integrated Security=True;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<customerData>(entity =>
            {
                entity.Property(e => e.latitude).HasMaxLength(50);

                entity.Property(e => e.longitude).HasMaxLength(50);
            });

            modelBuilder.Entity<depotData>(entity =>
            {
                entity.Property(e => e.latitude).HasMaxLength(50);

                entity.Property(e => e.longitude).HasMaxLength(50);

                entity.Property(e => e.name).HasMaxLength(50);
            });

            modelBuilder.Entity<kitType>(entity =>
            {
                entity.Property(e => e.name).HasMaxLength(50);
            });

            modelBuilder.Entity<vehicleData>(entity =>
            {
                entity.Property(e => e.name).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
