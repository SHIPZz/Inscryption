namespace Code.Features.Enemy.Services
{
    public interface IEnemyFactory
    {
        GameEntity CreateEnemy(int baseHealth);
    }
}

