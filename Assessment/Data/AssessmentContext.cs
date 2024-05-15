using System;
using System.Collections.Generic;
using Assessment.Models;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Data;

public partial class AssessmentContext : DbContext
{
    public AssessmentContext(DbContextOptions<AssessmentContext> options)
        : base(options)
    {
    }



    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PRIMARY");

            entity.ToTable("products");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.VideoId).HasName("PRIMARY");

            entity.ToTable("videos");

            entity.HasIndex(e => e.ProductId, "ProductId");

            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.Videos)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("videos_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
