using System.Linq;
using Code.Common;
using Code.Features.Board.Extensions;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class PlaceEnemyCardsSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _slots;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _placeCardRequests;

        public PlaceEnemyCardsSystem(GameContext game)
        {
            _game = game;
            _slots = game.GetGroup(GameMatcher.BoardSlot);
            _enemies = game.GetGroup(GameMatcher.Enemy);
            _placeCardRequests = game.GetGroup(GameMatcher.PlaceCardRequest);
        }

        public void Execute()
        {
            // Если есть запросы на размещение, ждем их обработки
            if (_placeCardRequests.count > 0)
                return;

            foreach (GameEntity enemy in _enemies)
            {
                if (!enemy.isEnemyTurn)
                    continue;

                // Если уже разместил карту в этом ходу - не размещаем еще
                if (enemy.hasCardsPlacedThisTurn && enemy.CardsPlacedThisTurn > 0)
                    return;

                // Если нет карт в руке - не размещаем
                if (enemy.CardsInHand.Count == 0)
                    return;

                // Ищем свободный слот
                var enemySlots = _slots.GetOwnedSlots(enemy.Id);
                foreach (var slot in enemySlots)
                {
                    bool isOccupied = slot.isOccupied || (slot.hasOccupiedBy && slot.OccupiedBy >= 0);
                    if (isOccupied)
                        continue;

                    // Размещаем первую карту из руки в первый свободный слот
                    int cardId = enemy.CardsInHand.First();
                    CreateEntity
                        .Request()
                        .AddPlaceCardRequest(cardId, slot.Id);
                    
                    return;
                }
            }
        }
    }
}
