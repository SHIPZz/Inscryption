using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Game.Systems
{
    public class CheckVictorySystem : IExecuteSystem
    {
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;
        private bool _gameEnded = false;

        public CheckVictorySystem(IHeroProvider heroProvider, IEnemyProvider enemyProvider)
        {
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;
        }

        public void Execute()
        {
            if (_gameEnded)
                return;

            GameEntity hero = _heroProvider.GetHero();
            GameEntity enemy = _enemyProvider.GetEnemy();

            if (hero == null || enemy == null)
                return;

            if (hero.Hp <= 0)
            {
                Debug.Log("=== GAME OVER: Enemy Wins! ===");
                _gameEnded = true;
                hero.isDestructed = true;
            }
            else if (enemy.Hp <= 0)
            {
                Debug.Log("=== VICTORY: Hero Wins! ===");
                _gameEnded = true;
                enemy.isDestructed = true;
            }
        }
    }
}

