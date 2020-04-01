using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace GrowIoT.Extensions
{
    public static class SqLiteExtensions
    {
        public static async Task InitDb(this IGrowDataContext context, bool seed = false, int seedCount = 5)
        {
            if(!(context is GrowDbContext dbContext))
                return;

            var result = await dbContext.Database.EnsureCreatedAsync();
    
            if (result && seed)
            {
                //TODO add seed data
            }
        }
    }
}
