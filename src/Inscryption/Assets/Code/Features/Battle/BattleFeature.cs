using Code.Features.Battle.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Battle
{
    public class BattleFeature : Feature
    {
        public BattleFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<CreateAttacksOnEndTurnSystem>());
            Add(systemFactory.Create<ProcessAttackRequestSystem>());
            Add(systemFactory.Create<AnimateOnAttackSystem>());
        }
    }
}

