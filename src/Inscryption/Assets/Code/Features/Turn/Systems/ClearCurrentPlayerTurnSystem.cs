using Code.Features.Turn.Extensions;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class ClearCurrentPlayerTurnSystem : IInitializeSystem
    {
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;

        public ClearCurrentPlayerTurnSystem(GameContext game)
        {
            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
        }

        public void Initialize()
        {
            GameEntity currentPlayer = TurnExtensions.GetCurrentPlayer(_heroes, _enemies);

            if (currentPlayer == null)
                return;

            if (currentPlayer.isHero)
                currentPlayer.isHeroTurn = false;
            else
                currentPlayer.isEnemyTurn = false;
        }
    }
}

