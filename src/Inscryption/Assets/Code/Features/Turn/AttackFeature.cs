using Code.Features.Turn.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Turn
{
    public class AttackFeature : Feature
    {
        public AttackFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<ScheduleAttacksSystem>());
            Add(systemFactory.Create<TransitionFromAttackSystem>());
        }
    }
}

