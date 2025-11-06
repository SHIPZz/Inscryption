using Code.Features.Turn.Extensions;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class ClearCurrentPlayerTurnSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;
        private bool _cleared;

        public ClearCurrentPlayerTurnSystem(GameContext game)
        {
            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
        }

        public void Execute()
        {
            if (_cleared)
                return;

            GameEntity currentPlayer = TurnExtensions.GetCurrentPlayer(_heroes, _enemies);

            if (currentPlayer == null)
                return;

            _cleared = true;
            if (currentPlayer.isHero)
                currentPlayer.isHeroTurn = false;
            else
                currentPlayer.isEnemyTurn = false;
        }
    }
}

