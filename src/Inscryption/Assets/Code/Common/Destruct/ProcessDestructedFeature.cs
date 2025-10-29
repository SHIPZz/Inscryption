using Code.Features.Death;
using Code.Infrastructure.Systems;

namespace Code.Common.Destruct
{
    public sealed class ProcessDestructedFeature : Feature
    {
        public ProcessDestructedFeature(ISystemFactory system)
        {
            Add(system.Create<DeathFeature>());
            Add(system.Create<SelfDestructTimerSystem>());
            Add(system.Create<CleanupGameDestructedViewSystem>());
            Add(system.Create<CleanupMetaDestructedSystem>());
            Add(system.Create<CleanupGameDestructedSystem>());
        }
    }
}