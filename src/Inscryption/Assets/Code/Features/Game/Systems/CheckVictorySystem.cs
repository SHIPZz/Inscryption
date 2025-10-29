using Code.Common.Extensions;
using Entitas;
using UnityEngine;

namespace Code.Features.Game.Systems
{
    public class CheckVictorySystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _gameEnds;
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;

        public CheckVictorySystem(GameContext game)
        {
            _game = game;
            _gameEnds = game.GetGroup(GameMatcher.GameEnd);
            _heroes = game.GetGroup(GameMatcher.AllOf(GameMatcher.Hero,GameMatcher.Hp));
            _enemies =game.GetGroup(GameMatcher.AllOf(GameMatcher.Enemy,GameMatcher.Hp));
        }

        public void Execute()
        {
            if (_gameEnds.count > 0)
                return;

            foreach (var hero in _heroes)
            foreach (var enemy in _enemies)
            {
                if (hero == null || hero.isDestructed || enemy == null || enemy.isDestructed) 
                    _game.CreateEntity().isGameEnd = true;

                if (_gameEnds.count > 0)
                    _game.CreateEntity().AddGameEndRequest(newHeroWon: hero.Hp > 0, hero.Hp, enemy.Hp)
                        .With(x => x.isRequest = true)
                        ;
            }
        }
    }
}