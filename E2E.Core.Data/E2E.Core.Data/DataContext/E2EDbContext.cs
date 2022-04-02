using System;
using System.Collections.Generic;
using System.Text;
using E2E.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace E2E.Core.Data.DataContext
{
  public class E2EDbContext : DbContext
  {
    public E2EDbContext(DbContextOptions<E2EDbContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      //Configure default schema
      modelBuilder.HasDefaultSchema("dbo");

      modelBuilder.Entity<EntityType>().ToTable("EntityType", "dbo").HasKey(s => new { s.EntityTypeID });
    }

    //entities
    public DbSet<EntityType> Entities { get; set; }
  }
}
