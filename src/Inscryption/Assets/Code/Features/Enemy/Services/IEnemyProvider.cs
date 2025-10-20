using Entitas;

namespace Code.Features.Enemy.Services
{
    public interface IEnemyProvider
    {
        GameEntity GetEnemy();
        GameEntity GetActiveEnemy();
        bool IsEnemyActive();
    }
}

