using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace HackItApi.Models
{
    public class HackContext : DbContext
    {
        public HackContext(DbContextOptions<HackContext> options)
            : base(options)
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;
            //
            optionsBuilder.UseMySql("server=167.71.46.107;port=3306;database=hackitall2020!;uid=admin;password=GreenMood2020!",
                new MySqlServerVersion(new Version(14, 14, 21)),
                mySqlOptions => mySqlOptions
                    .CharSetBehavior(CharSetBehavior.NeverAppend));
        }

        public DbSet<Item> Items { get; set; }
    }
}
