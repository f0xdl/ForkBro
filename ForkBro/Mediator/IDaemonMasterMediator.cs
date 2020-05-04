using System.Collections.Generic;
using ForkBro.Common;

namespace ForkBro.Mediator
{
    public interface IDaemonMasterMediator
    {
        int CountDaemons { get; set; }

        void AddFork(List<Fork> forks);
        PoolRaw GetNextPool();
        void UpdateDaemonMasterStatus();
    }
}