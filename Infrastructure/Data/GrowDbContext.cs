using System;
using System.Linq;
using FourTwenty.IoT.Connect.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class GrowDbContext : DbContext
    {
        public virtual DbSet<GrowBox> Boxes { get; set; }
        public virtual DbSet<GrowBoxModule> Modules { get; set; }
        public virtual DbSet<ModuleRule> Rules { get; set; }

        public GrowDbContext(DbContextOptions<GrowDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<GrowBox>().HasMany(x => x.Modules).WithOne(x => x.GrowBox)
                .HasForeignKey(x => x.GrowBoxId);

            modelBuilder.Entity<GrowBoxModule>((builder) =>
            {
                builder.Property(d => d.Pins).HasConversion(
                    v => string.Join(';', v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());

                builder.HasMany(x => x.Rules).WithOne(x => x.GrowBoxModule)
                    .HasForeignKey(x => x.GrowBoxModuleId);
            });
            modelBuilder.Entity<ModuleRule>().HasOne(x => x.GrowBoxModule).WithMany(x => x.Rules)
                .HasForeignKey(x => x.GrowBoxModuleId);
            
            modelBuilder.Entity<GrowBox>().HasData(new GrowBox() { Id = Constants.BoxId, Title = "My GrowBox" });
        }
    }
}
