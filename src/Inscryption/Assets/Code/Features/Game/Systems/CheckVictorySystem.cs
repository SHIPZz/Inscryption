using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Game.Systems
{
    public class CheckVictorySystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;
        private bool _gameEnded = false;

        public CheckVictorySystem(GameContext game, IHeroProvider heroProvider, IEnemyProvider enemyProvider)
        {
            _game = game;
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
            {
                _gameEnded = true;
                return;
            }

            if (hero.Hp <= 0)
            {
                Debug.Log($"=== GAME OVER: Enemy Wins! === (Hero HP: {hero.Hp}, Enemy HP: {enemy.Hp})");
                _gameEnded = true;

                _game.CreateEntity().AddGameEndRequest(false, hero.Hp, enemy.Hp);
            }
            else if (enemy.Hp <= 0)
            {
                Debug.Log($"=== VICTORY: Hero Wins! === (Hero HP: {hero.Hp}, Enemy HP: {enemy.Hp})");
                _gameEnded = true;

                _game.CreateEntity().AddGameEndRequest(true, hero.Hp, enemy.Hp);
            }
        }
    }
}

