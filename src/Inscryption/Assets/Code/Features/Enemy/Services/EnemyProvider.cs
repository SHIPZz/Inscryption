using Code.Common.Services;
using Entitas;

namespace Code.Features.Enemy.Services
{
    public class EnemyProvider : PlayerProvider, IEnemyProvider
    {
        public EnemyProvider(GameContext game) 
            : base(game, GameMatcher.Enemy, GameMatcher.EnemyTurn)
        {
        }

        public GameEntity GetEnemy() => GetPlayer();
        
        public GameEntity GetActiveEnemy() => GetActivePlayer();
        
        public bool IsEnemyActive() => IsPlayerActive();
    }
}

