using FourTwenty.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Entities
{
    public class ModuleHistoryItem : BaseEntity<int>
    {
        public int ModuleId { get; set; }
        /// <summary>
        /// Date in UTC
        /// </summary>
        public DateTime Date { get; set; }
        public string Data { get; set; }
    }
}
