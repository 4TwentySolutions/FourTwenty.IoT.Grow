using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrowIoT.Interfaces
{
    public interface IHubService
    {
        Task SendMessage(string key, params object[] value);
    }
}
