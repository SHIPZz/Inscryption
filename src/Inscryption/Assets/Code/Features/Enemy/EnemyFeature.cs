using Code.Features.Enemy.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Enemy
{
    public class EnemyFeature : Feature
    {
        public EnemyFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<ProcessEnemyTurnSystem>());
        }
    }
}

