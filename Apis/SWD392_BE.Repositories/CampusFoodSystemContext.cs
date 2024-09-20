using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SWD392_BE.Repositories.Entities;

namespace SWD392_BE.Repositories;

public partial class CampusFoodSystemContext : DbContext
{
    public CampusFoodSystemContext()
    {
    }

    public CampusFoodSystemContext(DbContextOptions<CampusFoodSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<Campus> Campuses { get; set; }

    public virtual DbSet<Food> Foods { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<StoreSession> StoreSessions { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionString());
    }

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:LocalDB"];
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Area__3214EC076EB53C8C");

            entity.ToTable("Area");

            entity.HasIndex(e => e.AreaId, "UQ__Area__70B82049F6FCECD5").IsUnique();

            entity.Property(e => e.AreaId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("date");
            entity.Property(e => e.DeletedBy).IsUnicode(false);
            entity.Property(e => e.DeletedDate).HasColumnType("date");
            entity.Property(e => e.ModifiedBy).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("date");
        });

        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Campus__3214EC07BC3ED06D");

            entity.ToTable("Campus");

            entity.HasIndex(e => e.CampusId, "UQ__Campus__FD598DD7D51C7835").IsUnique();

            entity.Property(e => e.AreaId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CampusId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("date");
            entity.Property(e => e.DeletedBy).IsUnicode(false);
            entity.Property(e => e.DeletedDate).HasColumnType("date");
            entity.Property(e => e.ModifiedBy).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("date");

            entity.HasOne(d => d.Area).WithMany(p => p.Campuses)
                .HasPrincipalKey(p => p.AreaId)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Campus_Area");
        });

        modelBuilder.Entity<Food>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Food__3214EC07A65BB656");

            entity.ToTable("Food");

            entity.HasIndex(e => e.FoodId, "UQ__Food__856DB3EA0330DB7B").IsUnique();

            entity.Property(e => e.CreatedBy).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("date");
            entity.Property(e => e.DeletedBy).IsUnicode(false);
            entity.Property(e => e.DeletedDate).HasColumnType("date");
            entity.Property(e => e.FoodId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Image).IsUnicode(false);
            entity.Property(e => e.ModifiedBy).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("date");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.StoreId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Store).WithMany(p => p.Foods)
                .HasPrincipalKey(p => p.StoreId)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Food_Store");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC07BFEB8954");

            entity.ToTable("Order");

            entity.HasIndex(e => e.OrderId, "UQ__Order__C3905BCEA7709ECD").IsUnique();

            entity.Property(e => e.CreatedBy).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("date");
            entity.Property(e => e.ModifiedBy).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SessionId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StoreId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TransactionId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Session).WithMany(p => p.Orders)
                .HasPrincipalKey(p => p.SessionId)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Session");

            entity.HasOne(d => d.Store).WithMany(p => p.Orders)
                .HasPrincipalKey(p => p.StoreId)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Store");

            entity.HasOne(d => d.Transaction).WithMany(p => p.Orders)
                .HasPrincipalKey(p => p.TransactionId)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Transaction");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasPrincipalKey(p => p.UserId)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_User");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderDet__3214EC07CCC13454");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.FoodId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Food).WithMany(p => p.OrderDetails)
                .HasPrincipalKey(p => p.FoodId)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Food");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasPrincipalKey(p => p.OrderId)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Order");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Session__3214EC07BFAABFCA");

            entity.ToTable("Session");

            entity.HasIndex(e => e.SessionId, "UQ__Session__C9F49291045FF8CF").IsUnique();

            entity.Property(e => e.CreatedBy).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("date");
            entity.Property(e => e.DeletedBy).IsUnicode(false);
            entity.Property(e => e.DeletedDate).HasColumnType("date");
            entity.Property(e => e.ModifiedBy).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("date");
            entity.Property(e => e.SessionId)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Store__3214EC0706D36287");

            entity.ToTable("Store");

            entity.HasIndex(e => e.StoreId, "UQ__Store__3B82F1008604BFB3").IsUnique();

            entity.Property(e => e.AreaId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("date");
            entity.Property(e => e.DeletedBy).IsUnicode(false);
            entity.Property(e => e.DeletedDate).HasColumnType("date");
            entity.Property(e => e.ModifiedBy).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("date");
            entity.Property(e => e.Phone)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.StoreId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Area).WithMany(p => p.Stores)
                .HasPrincipalKey(p => p.AreaId)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Store_Area");
        });

        modelBuilder.Entity<StoreSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StoreSes__3214EC078277F799");

            entity.ToTable("StoreSession");

            entity.Property(e => e.SessionId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StoreId)
                .HasMaxLength(50)
                .IsUnicode(false);


            entity.HasOne(d => d.Session).WithMany(p => p.StoreSessions)
                .HasPrincipalKey(p => p.SessionId)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StoreSession_Session");

            entity.HasOne(d => d.Store).WithMany(p => p.StoreSessions)
                .HasPrincipalKey(p => p.StoreId)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StoreSession_Store");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Token__1788CC4C60AFD7FC");

            entity.ToTable("Token");

            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AccessToken).IsUnicode(false);
            entity.Property(e => e.ExpiredTime).HasColumnType("datetime");
            entity.Property(e => e.RefreshToken).IsUnicode(false);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC072D632823");

            entity.ToTable("Transaction");

            entity.HasIndex(e => e.TransactionId, "UQ__Transact__B1E731540C7B27D0").IsUnique();

            entity.Property(e => e.CreatedBy).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("date");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasPrincipalKey(p => p.UserId)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC076684B818");

            entity.ToTable("User");

            entity.HasIndex(e => e.UserId, "UQ__User__1788CC4D7463EE33").IsUnique();

            entity.Property(e => e.CampusId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("date");
            entity.Property(e => e.DeletedBy).IsUnicode(false);
            entity.Property(e => e.DeletedDate).HasColumnType("date");
            entity.Property(e => e.Email).IsUnicode(false);
            entity.Property(e => e.ModifiedBy).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("date");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Campus).WithMany(p => p.Users)
                .HasPrincipalKey(p => p.CampusId)
                .HasForeignKey(d => d.CampusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Campus");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
