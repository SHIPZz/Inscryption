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
        private readonly System.Collections.Generic.List<GameEntity> _buffer = new(32);

        public ProcessPlaceCardRequestSystem(GameContext game, IConfigService configService)
        {
            _game = game;
            _requests = game.GetGroup(GameMatcher.PlaceCardRequest);
            _gameConfig = configService.GetConfig<GameConfig>();
        }

        public void Execute()
        {
            foreach (GameEntity request in _requests.GetEntities(_buffer))
            {
                int cardId = request.placeCardRequest.CardId;
                int slotId = request.placeCardRequest.SlotId;

                UnityEngine.Debug.Log($"[ProcessPlaceCardRequestSystem] Processing request: card {cardId} -> slot {slotId}");

                GameEntity card = _game.GetEntityWithId(cardId);
                GameEntity slot = _game.GetEntityWithId(slotId);

                if (!IsPlacementAllowed(card, slot))
                {
                    UnityEngine.Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Placement not allowed for card {cardId} -> slot {slotId}");
                    request.Destroy();
                    continue;
                }

                GameEntity owner = _game.GetEntityWithId(card.CardOwner);

                UnityEngine.Debug.Log($"[ProcessPlaceCardRequestSystem] Placing card {cardId} on slot {slotId} for owner {owner.Id}");
                SetupPlacing(card, slot);

                owner.CardsInHand.Remove(cardId);
                owner.PlacedCards.Add(cardId);
                owner.ReplaceCardsPlacedThisTurn(owner.CardsPlacedThisTurn + 1);

                request.Destroy();
            }
        }

        private bool IsPlacementAllowed(GameEntity card, GameEntity slot)
        {
            if (card == null || slot == null)
            {
                UnityEngine.Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Card or slot is null");
                return false;
            }

            if (!card.isCard || !card.isInHand || !slot.isBoardSlot)
            {
                UnityEngine.Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Invalid card or slot state: isCard={card.isCard}, isInHand={card.isInHand}, isBoardSlot={slot.isBoardSlot}");
                return false;
            }

            GameEntity owner = _game.GetEntityWithId(card.CardOwner);
            if (owner == null)
            {
                UnityEngine.Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Owner not found for card {card.Id}");
                return false;
            }

            bool ownerTurn = owner.isHero ? owner.isHeroTurn : owner.isEnemy && owner.isEnemyTurn;
            if (!ownerTurn)
            {
                UnityEngine.Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Not owner's turn: isHero={owner.isHero}, isHeroTurn={owner.isHeroTurn}, isEnemy={owner.isEnemy}, isEnemyTurn={owner.isEnemyTurn}");
                return false;
            }

            var maxCardsPerTurn = _gameConfig.GameBalance.MaxCardsPlacedPerTurn;
            if (!owner.hasCardsPlacedThisTurn || owner.CardsPlacedThisTurn >= maxCardsPerTurn)
            {
                UnityEngine.Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Max cards per turn reached: hasCardsPlacedThisTurn={owner.hasCardsPlacedThisTurn}, CardsPlacedThisTurn={owner.CardsPlacedThisTurn}, max={maxCardsPerTurn}");
                return false;
            }

            if (slot.isOccupied || slot.OccupiedBy >= 0)
            {
                UnityEngine.Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Slot is occupied: isOccupied={slot.isOccupied}, OccupiedBy={slot.OccupiedBy}");
                return false;
            }

            if (!slot.hasSlotOwner || slot.SlotOwner != owner.Id)
            {
                UnityEngine.Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Slot owner mismatch: hasSlotOwner={slot.hasSlotOwner}, SlotOwner={slot.SlotOwner}, ownerId={owner.Id}");
                return false;
            }

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