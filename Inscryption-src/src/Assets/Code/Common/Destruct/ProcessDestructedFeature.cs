using Code.Common.Systems.Destruct;
using Code.Infrastructure.Systems;

namespace Code.Common
{
    public sealed class ProcessDestructedFeature : Feature
    {
        public ProcessDestructedFeature(ISystemFactory system)
        {
            Add(system.Create<SelfDestructTimerSystem>());
            
            //
            // Add(system.Create<CleanupGameDestructedViewSystem>());
            // Add(system.Create<CleanupMetaDestructedSystem>());
            //
            // Add(system.Create<CleanupGameDestructedSystem>());
        }
    }
}