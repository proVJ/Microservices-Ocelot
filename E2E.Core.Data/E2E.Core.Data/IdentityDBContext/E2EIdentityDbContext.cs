using System;
using System.Collections.Generic;
using System.Text;
using E2E.Core.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E2E.Core.Data.DataContext
{
  public class E2EIdentityDbContext : IdentityDbContext<ApplicationUser>
  {
    public E2EIdentityDbContext(DbContextOptions<E2EIdentityDbContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
    }
  }

  public class ApplicationUser : IdentityUser
  {
    public string DOB { get; set; }
    public string Note { get; set; }
  }
}
