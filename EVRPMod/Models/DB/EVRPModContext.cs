﻿using System;
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

        public virtual DbSet<AlgorithmSettings> AlgorithmSettings { get; set; }
        public virtual DbSet<AverageRoadIntensityTable> AverageRoadIntensityTable { get; set; }
        public virtual DbSet<AverageSpeedTable> AverageSpeedTable { get; set; }
        public virtual DbSet<RoadQualityTable> RoadQualityTable { get; set; }
        public virtual DbSet<costTable> costTable { get; set; }
        public virtual DbSet<customerData> customerData { get; set; }
        public virtual DbSet<depotData> depotData { get; set; }
        public virtual DbSet<kitType> kitType { get; set; }
        public virtual DbSet<parametersKiniRayfaMethods> parametersKiniRayfaMethods { get; set; }
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
            modelBuilder.Entity<AlgorithmSettings>(entity =>
            {
                entity.HasKey(e => e.variable);

                entity.Property(e => e.variable).HasMaxLength(50);
            });

            modelBuilder.Entity<AverageRoadIntensityTable>(entity =>
            {
                entity.Property(e => e.id).ValueGeneratedNever();
            });

            modelBuilder.Entity<AverageSpeedTable>(entity =>
            {
                entity.Property(e => e.id).ValueGeneratedNever();
            });

            modelBuilder.Entity<RoadQualityTable>(entity =>
            {
                entity.Property(e => e.id).ValueGeneratedNever();
            });

            modelBuilder.Entity<costTable>(entity =>
            {
                entity.Property(e => e.id).ValueGeneratedNever();
            });

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

            modelBuilder.Entity<parametersKiniRayfaMethods>(entity =>
            {
                entity.HasKey(e => e.Criterion);

                entity.Property(e => e.Criterion).HasMaxLength(50);

                entity.Property(e => e.AverageValueFor__Values25).HasColumnName(@"AverageValueFor
Values25");

                entity.Property(e => e.AverageValueFor__Values50).HasColumnName(@"AverageValueFor
Values50");

                entity.Property(e => e.AverageValueFor__Values75).HasColumnName(@"AverageValueFor
Values75");

                entity.Property(e => e.ValueFor__WeightComparison__Criteria).HasColumnName(@"ValueFor
WeightComparison
Criteria");
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
