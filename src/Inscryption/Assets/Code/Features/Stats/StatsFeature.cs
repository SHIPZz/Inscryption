using Code.Features.Stats.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Stats
{
    public class StatsFeature : Feature
    {
        public StatsFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<ApplyHpFromStatsSystem>());
            Add(systemFactory.Create<AnimateOnDamageSystem>());
        }
    }
}