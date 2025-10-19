using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Features.Board.Systems
{
    public class ProcessPlaceCardRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _placeRequests;
        private readonly List<GameEntity> _buffer = new(8);

        private const int MaxCardsPerTurn = 1;

        public ProcessPlaceCardRequestSystem(GameContext game)
        {
            _game = game;

            _placeRequests = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.PlaceCardRequest));
        }

        public void Execute()
        {
            foreach (GameEntity request in _placeRequests.GetEntities(_buffer))
            {
                int cardId = request.placeCardRequest.CardId;
                int slotId = request.placeCardRequest.SlotId;

                GameEntity card = _game.GetEntityWithId(cardId);
                GameEntity slot = _game.GetEntityWithId(slotId);

                if (!ValidatePlacement(card, slot, out string error))
                {
                    Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Invalid placement: {error}");
                    request.isDestructed = true;
                    continue;
                }

                GameEntity owner = _game.GetEntityWithId(card.CardOwner);
                
                if (owner != null && owner.hasCardsPlacedThisTurn && owner.CardsPlacedThisTurn >= MaxCardsPerTurn)
                {
                    Debug.LogWarning($"[ProcessPlaceCardRequestSystem] Player {owner.Id} already placed {MaxCardsPerTurn} card(s) this turn");
                    request.isDestructed = true;
                    continue;
                }

                PlaceCardOnBoard(card, slot, owner);

                request.isDestructed = true;
            }
        }

        private bool ValidatePlacement(GameEntity card, GameEntity slot, out string error)
        {
            if (card == null)
            {
                error = "Card not found";
                return false;
            }

            if (slot == null)
            {
                error = "Slot not found";
                return false;
            }

            if (!card.isInHand)
            {
                error = $"Card {card.Id} is not in hand";
                return false;
            }

            if (!slot.isBoardSlot)
            {
                error = $"Entity {slot.Id} is not a board slot";
                return false;
            }

            if (slot.OccupiedBy != -1)
            {
                error = $"Slot {slot.Id} is already occupied by card {slot.OccupiedBy}";
                return false;
            }

            if (slot.SlotOwner != card.CardOwner)
            {
                error = $"Slot {slot.Id} belongs to player {slot.SlotOwner}, but card {card.Id} belongs to {card.CardOwner}";
                return false;
            }

            error = null;
            return true;
        }

        private void PlaceCardOnBoard(GameEntity card, GameEntity slot, GameEntity owner)
        {
            card.isInHand = false;
            card.isOnBoard = true;
            card.AddLane(slot.SlotLane);

            slot.ReplaceOccupiedBy(card.Id);

            if (owner != null && owner.hasCardsInHand)
            {
                owner.CardsInHand.Remove(card.Id);
            }

            if (owner != null)
            {
                if (owner.hasCardsPlacedThisTurn)
                    owner.ReplaceCardsPlacedThisTurn(owner.CardsPlacedThisTurn + 1);
                else
                    owner.AddCardsPlacedThisTurn(1);
            }

            Debug.Log($"[ProcessPlaceCardRequestSystem] Card {card.Id} placed on lane {card.Lane} (slot {slot.Id})");
        }
    }
}

