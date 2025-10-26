using System.Collections.Generic;
using Code.Features.Hero.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Board.Systems
{
    public class ProcessPlaceCardRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new();
        private IHeroProvider _heroProvider;

        public ProcessPlaceCardRequestSystem(GameContext game, IHeroProvider heroProvider)
        {
            _heroProvider = heroProvider;
            _game = game;
            _requests = game.GetGroup(GameMatcher.PlaceCardRequest);
        }

        public void Execute()
        {
            foreach (GameEntity request in _requests.GetEntities(_buffer))
            {
                int cardId = request.placeCardRequest.CardId;
                int slotId = request.placeCardRequest.SlotId;

                GameEntity card = _game.GetEntityWithId(cardId);
                GameEntity slot = _game.GetEntityWithId(slotId);

                if (card != null && slot != null && slot.hasWorldPosition)
                {
                    GameEntity cardOwner = _game.GetEntityWithId(card.CardOwner);

                    SetupPlacing(card, slot);

                    cardOwner.CardsInHand.Remove(cardId);
                    cardOwner.PlacedCards.Add(cardId);

                    Debug.Log(
                        $"[ProcessPlaceCardRequestSystem] Player {cardOwner.Id} placed card {cardId}. Switching turn...");
                }

                request.Destroy();
            }
        }

        private static void SetupPlacing(GameEntity card, GameEntity slot)
        {
            card.isStatic = false;
            card.isSelected = false;
            card.isSelectionAvailable = false;

            card.ReplaceParent(slot.Transform);
            card.ReplaceLocalPosition(Vector3.zero);
            card.ReplaceWorldPosition(Vector3.zero);
            card.ReplaceWorldRotation(Quaternion.Euler(0, 0, 0));
            card.ReplaceLocalRotation(Quaternion.Euler(0, 0, 0));
            card.ReplaceSlotLane(slot.SlotLane);
            card.ReplaceSlotId(slot.Id);


            slot.isOccupied = true;
            card.isPlaced = true;
        }
    }
}