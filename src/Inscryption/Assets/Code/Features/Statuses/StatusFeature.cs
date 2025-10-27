using Code.Features.Statuses.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Statuses
{
    public class StatusFeature : Feature
    {
        public StatusFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<ApplyDamageStatusSystem>());
            Add(systemFactory.Create<ApplyHpFromStatsSystem>());
            Add(systemFactory.Create<AnimateOnDamageSystem>());
        }
    }
}