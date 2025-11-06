using Entitas;

namespace Code.Features.Turn.Systems
{
    public class SetEnemyTurnSystem : IInitializeSystem
    {
        private readonly IGroup<GameEntity> _enemies;

        public SetEnemyTurnSystem(GameContext game)
        {
            _enemies = game.GetGroup(GameMatcher.Enemy);
        }

        public void Initialize()
        {
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
                break;
            }
        }
    }
}

