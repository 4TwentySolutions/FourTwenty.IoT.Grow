using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class HistoryDbContext : DbContext, IHistoryDataContext
    {
        public virtual DbSet<ModuleHistoryItem> Histories { get; set; }


        public HistoryDbContext(DbContextOptions<HistoryDbContext> options)
            : base(options)
        {

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
