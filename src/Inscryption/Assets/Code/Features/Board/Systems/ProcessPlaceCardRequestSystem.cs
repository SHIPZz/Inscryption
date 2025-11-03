using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Board.Systems
{
    public class ProcessPlaceCardRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _requests;
        private readonly GameConfig _gameConfig;

        public ProcessPlaceCardRequestSystem(GameContext game, IConfigService configService)
        {
            _game = game;
            _requests = game.GetGroup(GameMatcher.PlaceCardRequest);
            _gameConfig = configService.GetConfig<GameConfig>();
        }

        public void Execute()
        {
            foreach (GameEntity request in _requests)
            {
                int cardId = request.placeCardRequest.CardId;
                int slotId = request.placeCardRequest.SlotId;

                GameEntity card = _game.GetEntityWithId(cardId);
                GameEntity slot = _game.GetEntityWithId(slotId);

                if (!IsPlacementAllowed(card, slot))
                {
                    continue;
                }

                GameEntity owner = _game.GetEntityWithId(card.CardOwner);

                SetupPlacing(card, slot);

                owner.CardsInHand.Remove(cardId);
                owner.PlacedCards.Add(cardId);
                owner.ReplaceCardsPlacedThisTurn(owner.CardsPlacedThisTurn + 1);
            }
        }

        private bool IsPlacementAllowed(GameEntity card, GameEntity slot)
        {
            if (card == null || slot == null)
                return false;

            if (!card.isCard || !card.isInHand || !slot.isBoardSlot)
                return false;

            GameEntity owner = _game.GetEntityWithId(card.CardOwner);
            if (owner == null)
                return false;

            bool ownerTurn = owner.isHero ? owner.isHeroTurn : owner.isEnemy && owner.isEnemyTurn;
            if (!ownerTurn)
                return false;

            var maxCardsPerTurn = _gameConfig.GameBalance.MaxCardsPlacedPerTurn;
            if (!owner.hasCardsPlacedThisTurn || owner.CardsPlacedThisTurn >= maxCardsPerTurn)
                return false;

            if (slot.isOccupied || slot.OccupiedBy >= 0)
                return false;

            if (!slot.hasSlotOwner || slot.SlotOwner != owner.Id)
                return false;

            return true;
        }

        private void SetupPlacing(GameEntity card, GameEntity slot)
        {
            card.isStatic = false;
            card.isSelected = false;
            card.isSelectionAvailable = false;
            card.isInHand = false;
            card.isPlaced = true;
            card.isOnBoard = true;

            card.ReplaceParent(slot.Transform);
            card.ReplaceLocalPosition(Vector3.zero);
            card.ReplaceWorldPosition(slot.WorldPosition);
            card.ReplaceWorldRotation(slot.WorldRotation);
            card.ReplaceLocalRotation(Quaternion.identity);
            card.ReplaceSlotLane(slot.SlotLane);
            card.ReplaceSlotId(slot.Id);

            if (card.hasVisualTransform && card.VisualTransform != null)
            {
                card.VisualTransform.localPosition = Vector3.zero;
                card.VisualTransform.localRotation = Quaternion.identity;
            }

            slot.isOccupied = true;
            slot.ReplaceOccupiedBy(card.Id);
        }
    }
}