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
            if (HasPendingPlaceCardRequests())
                return;

            GameEntity enemy = FindEnemyInTurn();
            if (enemy == null || !CanEnemyPlaceCard(enemy))
                return;

            GameEntity freeSlot = FindFreeSlotForEnemy(enemy.Id);
            if (freeSlot == null)
                return;

            CreatePlaceCardRequest(enemy, freeSlot);
        }

        private bool HasPendingPlaceCardRequests()
        {
            return _placeCardRequests.count > 0;
        }

        private GameEntity FindEnemyInTurn()
        {
            foreach (GameEntity enemy in _enemies)
            {
                if (enemy.isEnemyTurn)
                    return enemy;
            }
            return null;
        }

        private bool CanEnemyPlaceCard(GameEntity enemy)
        {
            if (enemy.hasCardsPlacedThisTurn && enemy.CardsPlacedThisTurn > 0)
                return false;

            if (enemy.CardsInHand.Count == 0)
                return false;

            return true;
        }

        private GameEntity FindFreeSlotForEnemy(int enemyId)
        {
            var enemySlots = _slots.GetOwnedSlots(enemyId);
            foreach (var slot in enemySlots)
            {
                bool isOccupied = slot.isOccupied || (slot.hasOccupiedBy && slot.OccupiedBy >= 0);
                if (!isOccupied)
                    return slot;
            }
            return null;
        }

        private void CreatePlaceCardRequest(GameEntity enemy, GameEntity slot)
        {
            int cardId = enemy.CardsInHand.First();
            CreateEntity
                .Request()
                .AddPlaceCardRequest(cardId, slot.Id);
        }
    }
}
