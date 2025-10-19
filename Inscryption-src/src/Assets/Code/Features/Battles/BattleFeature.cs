using Code.Common;
using Code.Features.Statuses;
using Code.Infrastructure.Systems;
using Entitas;

namespace Code.Features.Battles
{
    public class BattleFeature : Feature
    {
        public BattleFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<StatusFeature>());
            Add(systemFactory.Create<ProcessDestructedFeature>());
        }
    }
}