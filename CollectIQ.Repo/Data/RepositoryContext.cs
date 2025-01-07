using CollectIQ.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
namespace CollectIQ.Repo.Data
{
    public class RepositoryContext : IdentityDbContext<User, Role, string>
    {

        public RepositoryContext()
        {
        }

        public RepositoryContext(DbContextOptions options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Item>()
                .HasOne(i => i.ItemType)
                .WithMany(it => it.Items)
                .HasForeignKey(i => i.ItemTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Watch>().ToTable("Watches");
            modelBuilder.Entity<Cologne>().ToTable("Colognes");
            modelBuilder.Entity<Sneaker>().ToTable("Sneakers");
            modelBuilder.Entity<Item>().ToTable("Items");


            modelBuilder.Entity<ItemType>().HasData(
                    new ItemType { Id = new System.Guid("f5b3e3d4-3b0d-4b5d-9b4d-1b3e4e6f7a8b"), Name = "Sneaker" },
                    new ItemType { Id = new System.Guid("f5b3e3d4-3b0d-4b5d-9b4d-1b3e4e6f7a8c"), Name = "Cologne" },
                    new ItemType { Id = new System.Guid("f5b3e3d4-3b0d-4b5d-9b4d-1b3e4e6f7a8d"), Name = "Watch" }
                );

            base.OnModelCreating(modelBuilder);

            foreach(var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(name: entity.GetTableName().ToTrimPrefix());
            }
            
        }
        
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Sneaker> Sneakers { get; set; }
        public DbSet<Cologne> Colognes { get; set; }
        public DbSet<Watch> Watches { get; set; }

    }

    public static class StringExensions
    {
        public static string ToTrimPrefix(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return Regex.Replace(input, @"^AspNet", "");
        }

    }
}
