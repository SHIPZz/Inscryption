using Code.Infrastructure.Level;
using Entitas;
using UnityEngine;

namespace Code.Features.Board.Systems
{
    public class ProcessPlaceCardRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly ILevelProvider _levelProvider;
        private readonly IGroup<GameEntity> _gamePlaceRequests;
        private readonly System.Collections.Generic.List<GameEntity> _gameRequestsBuffer = new System.Collections.Generic.List<GameEntity>();
        private readonly System.Collections.Generic.HashSet<int> _processedSlotIds = new System.Collections.Generic.HashSet<int>();

        public ProcessPlaceCardRequestSystem(GameContext game, ILevelProvider levelProvider)
        {
            _game = game;
            _levelProvider = levelProvider;
            _gamePlaceRequests = _game.GetGroup(GameMatcher.PlaceCardRequest);
        }

        public void Execute()
        {
            _processedSlotIds.Clear();

            foreach (GameEntity request in _gamePlaceRequests.GetEntities(_gameRequestsBuffer))
            {
                int cardId = request.placeCardRequest.CardId;
                int slotId = request.placeCardRequest.SlotId;
                GameEntity slot = _game.GetEntityWithId(slotId);
                GameEntity card = _game.GetEntityWithId(cardId);

                if (slot != null && slot.isBoardSlot && card != null && !_processedSlotIds.Contains(slotId) && CanPlaceCard(card, slot))
                {
                    PlaceCard(card, slot);
                    _processedSlotIds.Add(slotId);
                }

                request.isDestructed = true;
            }
        }

        private bool CanPlaceCard(GameEntity card, GameEntity slot)
        {
            if (!card.isInHand)
            {
                Debug.LogWarning($"[ProcessPlaceCardRequest] Card {card.Id} is not in hand");
                return false;
            }

            if (slot.OccupiedBy != -1)
            {
                Debug.LogWarning($"[ProcessPlaceCardRequest] Slot {slot.Id} is already occupied");
                return false;
            }

            if (card.CardOwner != slot.SlotOwner)
            {
                Debug.LogWarning($"[ProcessPlaceCardRequest] Card owner doesn't match slot owner");
                return false;
            }

            int ownerId = card.CardOwner;
            GameEntity owner = _game.GetEntityWithId(ownerId);

            if (owner == null)
            {
                Debug.LogWarning($"[ProcessPlaceCardRequest] Owner {ownerId} not found");
                return false;
            }

            if (owner.isHero && !owner.isHeroTurn)
            {
                Debug.LogWarning("[ProcessPlaceCardRequest] It's not hero's turn");
                return false;
            }

            if (owner.isEnemy && !owner.isEnemyTurn)
            {
                Debug.LogWarning("[ProcessPlaceCardRequest] It's not enemy's turn");
                return false;
            }

            if (owner.hasCardsPlacedThisTurn && owner.CardsPlacedThisTurn >= 1)
            {
                Debug.LogWarning($"[ProcessPlaceCardRequest] Player already placed {owner.CardsPlacedThisTurn} card(s) this turn");
                return false;
            }

            return true;
        }

        private void PlaceCard(GameEntity card, GameEntity slot)
        {
            int ownerId = card.CardOwner;
            GameEntity owner = _game.GetEntityWithId(ownerId);

            owner.CardsInHand.Remove(card.Id);

            if (owner.hasCardsPlacedThisTurn)
                owner.ReplaceCardsPlacedThisTurn(owner.CardsPlacedThisTurn + 1);
            else
                owner.AddCardsPlacedThisTurn(1);

            card.isInHand = false;
            card.isOnBoard = true;
            card.ReplaceLane(slot.SlotLane);
            slot.ReplaceOccupiedBy(card.Id);

            if (slot.hasWorldPosition)
            {
                card.ReplaceParent(_levelProvider.SlotsParent);
                card.ReplaceLocalPosition(slot.WorldPosition);
            }
            
            if (card.hasWorldPosition)
                card.RemoveWorldPosition();

            if (slot.hasWorldRotation)
            {
                card.ReplaceWorldRotation(slot.WorldRotation);
            }

            Debug.Log($"[ProcessPlaceCardRequest] Player {ownerId} placed card {card.Id}, total cards placed this turn: {owner.CardsPlacedThisTurn}");

            if (_game.hasSelectedCards && _game.SelectedCards.Count > 0 && _game.SelectedCards[0] == card.Id)
                _game.RemoveSelectedCards();
        }
    }
}
