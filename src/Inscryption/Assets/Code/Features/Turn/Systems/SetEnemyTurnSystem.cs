using Entitas;

namespace Code.Features.Turn.Systems
{
    public class SetEnemyTurnSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _enemies;
        private bool _set;

        public SetEnemyTurnSystem(GameContext game)
        {
            _enemies = game.GetGroup(GameMatcher.Enemy);
        }

        public void Execute()
        {
            if (_set)
                return;

            foreach (GameEntity enemy in _enemies)
            {
                if (enemy.isEnemyTurn)
                    return;

                enemy.isEnemyTurn = true;
                
                if (enemy.hasCardsPlacedThisTurn)
                    enemy.ReplaceCardsPlacedThisTurn(0);
                else
                    enemy.AddCardsPlacedThisTurn(0);
                
                UnityEngine.Debug.Log($"[SetEnemyTurnSystem] Set enemy {enemy.Id} turn, cardsInHand: {enemy.CardsInHand.Count}");
                _set = true;
                break;
            }
        }
    }
}

