using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProxyServer.Models;

namespace ProxyServer.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Fractal> Fractals { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
