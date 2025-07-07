using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ExpenseManager.DataAccessLayer.Entities;

namespace ExpenseManager.DataAccessLayer.Data;

public partial class WalletManagerDbContext : DbContext
{
    public WalletManagerDbContext()
    {
    }

    public WalletManagerDbContext(DbContextOptions<WalletManagerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Recurring> RecurringExpenses { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Limit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Categories)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("FK_Categories_Users");
            entity.HasOne(d => d.UpdateByNavigation)
                .WithMany() 
                .HasForeignKey(d => d.UpdateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categories_Users_UpdateBy");
    });

        modelBuilder.Entity<Recurring>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.RepeatInterval).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.RecurringExpenses)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RecurringExpenses_Categories");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.RecurringExpenses)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RecurringExpenses_Users");

            entity.HasOne(d => d.Wallet).WithMany(p => p.RecurringExpenses)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RecurringExpenses_Wallet");

           entity.HasOne(d => d.UpdateByNavigation)
                .WithMany()
                .HasForeignKey(d => d.UpdateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RecurringExpenses_Users_UpdateBy");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Categories");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Users");

            entity.HasOne(d => d.Wallet).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Wallet");

            entity.HasOne(d => d.UpdateByNavigation)
                .WithMany()
                .HasForeignKey(d => d.UpdateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Users_UpdateBy");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.RefreshToken).HasColumnType("nvarchar(max)");
            entity.Property(e => e.RefreshTokenExpiryTime).HasColumnType("datetime");
            entity.HasOne(d => d.UpdateByNavigation).WithMany()
                .HasForeignKey(d => d.UpdateBy)
                .HasConstraintName("FK_Users_Users_UpdateBy");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.ToTable("Wallet");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wallet_Users");

            entity.HasOne(d => d.UpdateByNavigation)
                .WithMany()
                .HasForeignKey(d => d.UpdateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wallet_Users_UpdateBy");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
