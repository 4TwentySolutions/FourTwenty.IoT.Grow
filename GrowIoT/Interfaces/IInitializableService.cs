using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrowIoT.Interfaces
{
    public interface IInitializableService
    {
        public bool IsInitialized { get; set; }

        void Initialize(InitializableOptions options);
    }

    public class InitializableOptions
    {

    }
}
