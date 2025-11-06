using Code.Features.Turn.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Turn
{
    public class EnemyTurnFeature : Feature
    {
        public EnemyTurnFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<SetEnemyTurnSystem>());
            Add(systemFactory.Create<TransitionToDrawSystem>());
        }
    }
}

