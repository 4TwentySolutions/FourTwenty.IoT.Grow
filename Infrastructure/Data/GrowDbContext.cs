﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class GrowDbContext : IdentityDbContext, IGrowDataContext
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

        public void Commit()
        {
            SaveChanges();
        }

        public async Task CommitAsync(CancellationToken token = new CancellationToken())
        {
            await SaveChangesAsync(token);
        }

        #region Not Implemented

        public Task BeginTransactionAsync(CancellationToken token = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public Task RollbackAsync(CancellationToken token = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
