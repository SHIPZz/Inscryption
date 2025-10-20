using Entitas;
using UnityEngine;

namespace Code.Features.Board.Systems
{
    public class ProcessPlaceCardRequestSystem : IExecuteSystem
    {
        private readonly InputContext _input;
        private readonly GameContext _game;
        private readonly IGroup<InputEntity> _requests;
        private readonly System.Collections.Generic.List<InputEntity> _buffer = new System.Collections.Generic.List<InputEntity>();

        public ProcessPlaceCardRequestSystem(InputContext input, GameContext game)
        {
            _input = input;
            _game = game;
            _requests = _input.GetGroup(InputMatcher.SlotClickRequest);
        }

        public void Execute()
        {
            foreach (InputEntity request in _requests.GetEntities(_buffer))
            {
                int slotId = request.SlotClickRequest;
                GameEntity slot = _game.GetEntityWithId(slotId);

                if (slot != null && slot.isBoardSlot && _game.hasSelectedCards && _game.SelectedCards.Count > 0)
                {
                    int selectedCardId = _game.SelectedCards[0];
                    GameEntity card = _game.GetEntityWithId(selectedCardId);

                    if (card != null && CanPlaceCard(card, slot))
                    {
                        PlaceCard(card, slot);
                        _game.RemoveSelectedCards();
                        Debug.Log($"[ProcessPlaceCardRequest] Card {selectedCardId} placed on slot {slotId}");
                    }
                }

                request.Destroy();
            }
        }

        private bool CanPlaceCard(GameEntity card, GameEntity slot)
        {
            if (!card.isInHand)
                return false;

            if (slot.OccupiedBy != -1)
                return false;

            if (card.CardOwner != slot.SlotOwner)
                return false;

            return true;
        }

        private void PlaceCard(GameEntity card, GameEntity slot)
        {
            int ownerId = card.CardOwner;
            GameEntity owner = _game.GetEntityWithId(ownerId);
            
            if (owner != null && owner.hasCardsInHand)
            {
                owner.CardsInHand.Remove(card.Id);
            }

            card.isInHand = false;
            card.isOnBoard = true;
            card.ReplaceLane(slot.SlotLane);
            slot.ReplaceOccupiedBy(card.Id);

            if (slot.hasWorldPosition)
            {
                card.ReplaceWorldPosition(slot.WorldPosition);
            }

            if (slot.hasWorldRotation)
            {
                card.ReplaceWorldRotation(slot.WorldRotation);
            }
        }
    }
}
