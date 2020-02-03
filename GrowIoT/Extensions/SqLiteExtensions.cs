using System.Threading.Tasks;
using Infrastructure.Data;

namespace GrowIoT.Extensions
{
    public static class SqLiteExtensions
    {
        public static async Task InitDb(this GrowDbContext context, bool seed = false, int seedCount = 5)
        {
            var result = await context.Database.EnsureCreatedAsync();
    
            if (seed)
            {
                //TODO add seed data
            }
        }
    }
}
