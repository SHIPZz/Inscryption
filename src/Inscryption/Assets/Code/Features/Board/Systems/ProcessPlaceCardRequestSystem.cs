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
                UnityEngine.Debug.Log($"[ProcessPlaceCardRequestSystem] Processing request: card={request.placeCardRequest.CardId}, slot={request.placeCardRequest.SlotId}");
                ProcessPlaceCardRequest(request);
            }
        }

        private void ProcessPlaceCardRequest(GameEntity request)
        {
            int cardId = request.placeCardRequest.CardId;
            int slotId = request.placeCardRequest.SlotId;

            GameEntity card = _game.GetEntityWithId(cardId);
            GameEntity slot = _game.GetEntityWithId(slotId);

            UnityEngine.Debug.Log($"[ProcessPlaceCardRequestSystem] Card={cardId}, Slot={slotId}, CardEntity={card != null}, SlotEntity={slot != null}");

            if (!ValidatePlacement(card, slot, cardId, slotId))
            {
                UnityEngine.Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Validation failed for card={cardId}, slot={slotId}");
                request.Destroy();
                return;
            }

            GameEntity owner = _game.GetEntityWithId(card.CardOwner);
            UnityEngine.Debug.Log($"[ProcessPlaceCardRequestSystem] Placing card {cardId} on slot {slotId} for owner {owner.Id}");
            PlaceCardOnSlot(card, slot, owner);
            UpdateOwnerCards(card, owner);
            request.Destroy();
        }

        private bool ValidatePlacement(GameEntity card, GameEntity slot, int cardId, int slotId)
        {
            if (!ValidateCardAndSlot(card, slot, cardId, slotId))
                return false;

            GameEntity owner = _game.GetEntityWithId(card.CardOwner);
            if (!ValidateOwner(owner, card))
                return false;

            if (!ValidateOwnerTurn(owner))
                return false;

            if (!ValidateCardsPlacedLimit(owner))
                return false;

            if (!ValidateSlotAvailability(slot))
                return false;

            if (!ValidateSlotOwnership(slot, owner))
                return false;

            return true;
        }

        private bool ValidateCardAndSlot(GameEntity card, GameEntity slot, int cardId, int slotId)
        {
            if (card == null || slot == null)
            {
                Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Card or slot is null");
                return false;
            }

            if (!card.isCard || !card.isInHand || !slot.isBoardSlot)
            {
                Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Invalid card or slot state: isCard={card.isCard}, isInHand={card.isInHand}, isBoardSlot={slot.isBoardSlot}");
                return false;
            }

            return true;
        }

        private bool ValidateOwner(GameEntity owner, GameEntity card)
        {
            if (owner == null)
            {
                Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Owner not found for card {card.Id}");
                return false;
            }
            return true;
        }

        private bool ValidateOwnerTurn(GameEntity owner)
        {
            bool ownerTurn = owner.isHero ? owner.isHeroTurn : owner.isEnemy && owner.isEnemyTurn;
            if (!ownerTurn)
            {
                Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Not owner's turn: isHero={owner.isHero}, isHeroTurn={owner.isHeroTurn}, isEnemy={owner.isEnemy}, isEnemyTurn={owner.isEnemyTurn}");
                return false;
            }
            return true;
        }

        private bool ValidateCardsPlacedLimit(GameEntity owner)
        {
            int maxCardsPerTurn = _gameConfig.GameBalance.MaxCardsPlacedPerTurn;
            if (!owner.hasCardsPlacedThisTurn || owner.CardsPlacedThisTurn >= maxCardsPerTurn)
            {
                Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Max cards per turn reached: hasCardsPlacedThisTurn={owner.hasCardsPlacedThisTurn}, CardsPlacedThisTurn={owner.CardsPlacedThisTurn}, max={maxCardsPerTurn}");
                return false;
            }
            return true;
        }

        private bool ValidateSlotAvailability(GameEntity slot)
        {
            if (slot.isOccupied || slot.OccupiedBy >= 0)
            {
                Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Slot is occupied: isOccupied={slot.isOccupied}, OccupiedBy={slot.OccupiedBy}");
                return false;
            }
            return true;
        }

        private bool ValidateSlotOwnership(GameEntity slot, GameEntity owner)
        {
            if (!slot.hasSlotOwner || slot.SlotOwner != owner.Id)
            {
                Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Slot owner mismatch: hasSlotOwner={slot.hasSlotOwner}, SlotOwner={slot.SlotOwner}, ownerId={owner.Id}");
                return false;
            }
            return true;
        }

        private void PlaceCardOnSlot(GameEntity card, GameEntity slot, GameEntity owner)
        {
            UpdateCardState(card);
            UpdateCardTransform(card, slot);
            UpdateCardVisualTransform(card);
            UpdateSlotOccupation(slot, card);
        }

        private void UpdateCardState(GameEntity card)
        {
            card.isStatic = false;
            card.isSelected = false;
            card.isSelectionAvailable = false;
            card.isInHand = false;
            card.isPlaced = true;
            card.isOnBoard = true;
        }

        private void UpdateCardTransform(GameEntity card, GameEntity slot)
        {
            card.ReplaceParent(slot.Transform);
            card.ReplaceLocalPosition(Vector3.zero);
            card.ReplaceWorldPosition(slot.WorldPosition);
            card.ReplaceWorldRotation(slot.WorldRotation);
            card.ReplaceLocalRotation(Quaternion.identity);
            card.ReplaceSlotLane(slot.SlotLane);
            card.ReplaceSlotId(slot.Id);
        }

        private void UpdateCardVisualTransform(GameEntity card)
        {
            if (card.hasVisualTransform && card.VisualTransform != null)
            {
                card.VisualTransform.localPosition = Vector3.zero;
                card.VisualTransform.localRotation = Quaternion.identity;
            }
        }

        private void UpdateSlotOccupation(GameEntity slot, GameEntity card)
        {
            slot.isOccupied = true;
            slot.ReplaceOccupiedBy(card.Id);
        }

        private void UpdateOwnerCards(GameEntity card, GameEntity owner)
        {
            owner.CardsInHand.Remove(card.Id);
            owner.PlacedCards.Add(card.Id);
            owner.ReplaceCardsPlacedThisTurn(owner.CardsPlacedThisTurn + 1);
        }
    }
}
