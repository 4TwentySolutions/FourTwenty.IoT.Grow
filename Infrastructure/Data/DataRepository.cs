using System.Threading.Tasks;
using FourTwenty.Core.Data.Models;

namespace Infrastructure.Data
{
    public class DataRepository<T, TKey> : TrackedEfRepository<T, TKey> where T : BaseEntity<TKey>
    {
        public DataRepository(GrowDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task UpdateAsync(T entity)
        {
            await base.UpdateAsync(entity);
            //DbContext.Entry(entity).State = EntityState.Detached;
        }

        public override async Task<T> AddAsync(T entity)
        {
            var item = await base.AddAsync(entity);
            //DbContext.Entry(item).State = EntityState.Detached;
            return item;
        }

        public override async Task<T> GetByIdAsync(TKey id)
        {
            var item = await base.GetByIdAsync(id);
            //DbContext.Entry(item).State = EntityState.Detached;
            return item;
        }
    }
    public class DataRepository<T> : TrackedEfRepository<T> where T : class
    {
        public DataRepository(GrowDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task UpdateAsync(T entity)
        {
            await base.UpdateAsync(entity);
            //DbContext.Entry(entity).State = EntityState.Detached;
        }

        public override async Task<T> AddAsync(T entity)
        {
            var item = await base.AddAsync(entity);
            //DbContext.Entry(entity).State = EntityState.Detached;
            return item;
        }

    }
}
