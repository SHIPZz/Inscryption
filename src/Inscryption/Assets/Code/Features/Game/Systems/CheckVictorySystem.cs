using Entitas;
using UnityEngine;

namespace Code.Features.Game.Systems
{
    //todo refactor this
    public class CheckVictorySystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;
        private bool _gameEnded = false;

        public CheckVictorySystem(GameContext game)
        {
            _game = game;
            _heroes = game.GetGroup(GameMatcher.AllOf(GameMatcher.Hero, GameMatcher.Hp));
            _enemies = game.GetGroup(GameMatcher.AllOf(GameMatcher.Enemy, GameMatcher.Hp));
        }

        public void Execute()
        {
            if (_gameEnded)
                return;

            foreach (GameEntity hero in _heroes)
            {
                if (hero.isDestructed)
                {
                    _gameEnded = true;

                    _game.CreateEntity().AddGameEndRequest(false, hero.Hp, hero.Hp);
                }
                
                foreach (GameEntity enemy in _enemies)
                {
                    if (enemy.isDestructed)
                    {
                        _gameEnded = true;

                        _game.CreateEntity().AddGameEndRequest(true, enemy.Hp, enemy.Hp);
                    }
                }
            }
        }
    }
}

