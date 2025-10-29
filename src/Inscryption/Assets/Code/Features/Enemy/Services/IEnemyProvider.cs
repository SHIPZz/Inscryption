namespace Code.Features.Enemy.Services
{
    public interface IEnemyProvider
    {
        GameEntity GetEnemy();
        bool IsEnemyActive();
    }
}

