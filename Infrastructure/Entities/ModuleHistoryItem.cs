using FourTwenty.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Entities
{
    public class ModuleHistoryItem : BaseEntity<int>
    {
        public int ModuleId { get; set; }
        //TODO it's not utc for now as it it's not DateTimeOffset If DateTimeOffset isn't working in sqlite - you can google solutions.
        /// <summary>
        /// Date in UTC
        /// </summary>
        public DateTime Date { get; set; }
        public string Data { get; set; }
    }
}
