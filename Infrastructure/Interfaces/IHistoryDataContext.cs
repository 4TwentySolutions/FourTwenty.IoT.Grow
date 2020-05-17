using System;
using FourTwenty.Core.Data.Interfaces;
using FourTwenty.IoT.Connect.Entities;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Interfaces
{
    public interface IHistoryDataContext : IDataContext, IDisposable
    {
        DbSet<ModuleHistoryItem> Histories { get; set; }
    }
}
