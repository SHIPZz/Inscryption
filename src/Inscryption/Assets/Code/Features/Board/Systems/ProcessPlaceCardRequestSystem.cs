using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Features.Board.Systems
{
    public class ProcessPlaceCardRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new();

        public ProcessPlaceCardRequestSystem(GameContext game)
        {
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
                    card.isStatic = false;
                    card.isSelected = false;
                    card.ReplaceParent(slot.Transform);
                    card.ReplaceLocalPosition(Vector3.zero);
                    card.ReplaceWorldPosition(Vector3.zero);
                    card.ReplaceWorldRotation(Quaternion.Euler(0,0,0));
                    card.ReplaceLocalRotation(Quaternion.Euler(0,0,0));
                }

                request.Destroy();
            }
        }
    }
}
