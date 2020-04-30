using FourTwenty.Core.Data.Interfaces;
using FourTwenty.IoT.Connect.Entities;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Interfaces
{
    public interface IHistoryDataContext : IDataContext
    {
        DbSet<ModuleHistoryItem> Histories { get; set; }
    }
}
