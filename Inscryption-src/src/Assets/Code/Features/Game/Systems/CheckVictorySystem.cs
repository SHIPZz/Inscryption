using Entitas;
using UnityEngine;

namespace Code.Features.Game.Systems
{
    public class CheckVictorySystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private bool _gameEnded = false;

        public CheckVictorySystem(GameContext game)
        {
            _game = game;
        }

        public void Execute()
        {
            if (_gameEnded)
                return;

            GameEntity hero = GetHero();
            GameEntity enemy = GetEnemy();

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

        private GameEntity GetHero()
        {
            foreach (GameEntity entity in _game.GetEntities(GameMatcher.Hero))
            {
                if (!entity.isDestructed)
                    return entity;
            }
            return null;
        }

        private GameEntity GetEnemy()
        {
            foreach (GameEntity entity in _game.GetEntities(GameMatcher.Enemy))
            {
                if (!entity.isDestructed)
                    return entity;
            }
            return null;
        }
    }
}

