using System;
using System.Collections.Generic;
using System.Text;
using FourTwenty.Core.Data.Interfaces;
using FourTwenty.IoT.Connect.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Interfaces
{
    public interface IGrowDataContext : IEfDataContext
    {
        DbSet<GrowBox> Boxes { get; set; }
        DbSet<GrowBoxModule> Modules { get; set; }
        DbSet<ModuleRule> Rules { get; set; }

    }
}
