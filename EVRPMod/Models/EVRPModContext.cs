using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EVRPMod
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

        public virtual DbSet<CustomerData> CustomerData { get; set; }
        public virtual DbSet<DepotData> DepotData { get; set; }
        public virtual DbSet<KitType> KitType { get; set; }
        public virtual DbSet<VehicleData> VehicleData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-IDS9RKI;Database=EVRPMod;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerData>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("customerData");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.KitType).HasColumnName("kitType");

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasMaxLength(50);

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DepotData>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("depotData");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasMaxLength(50);

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<KitType>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("kitType");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Weight).HasColumnName("weight");
            });

            modelBuilder.Entity<VehicleData>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("vehicleData");

                entity.Property(e => e.Capacity).HasColumnName("capacity");

                entity.Property(e => e.CostRoads).HasColumnName("costRoads");

                entity.Property(e => e.Depot).HasColumnName("depot");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.ServiceCost).HasColumnName("serviceCost");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
