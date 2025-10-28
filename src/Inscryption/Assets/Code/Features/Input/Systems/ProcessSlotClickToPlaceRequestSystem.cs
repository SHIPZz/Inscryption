using System.Collections.Generic;
using Code.Common.Extensions;
using Entitas;
using UnityEngine;

namespace Code.Features.Input.Systems
{
    public class ProcessSlotClickToPlaceRequestSystem : IExecuteSystem
    {
        private readonly InputContext _input;
        private readonly GameContext _game;
        private readonly IGroup<InputEntity> _requests;
        private readonly List<InputEntity> _buffer = new();
        private readonly IGroup<GameEntity> _selectedCards;

        public ProcessSlotClickToPlaceRequestSystem(InputContext input, GameContext game)
        {
            _input = input;
            _game = game;
            _requests = _input.GetGroup(InputMatcher.SlotClickRequest);

            _selectedCards = game.GetGroup(GameMatcher.AllOf(GameMatcher.Card, GameMatcher.Selected));
        }

        public void Execute()
        {
            foreach (InputEntity request in _requests.GetEntities(_buffer))
            foreach (GameEntity selectedCard in _selectedCards)
            {
                int slotId = request.SlotClickRequest;

                GameEntity slot = _game.GetEntityWithId(slotId);

                if (slot != null && slot.isBoardSlot)
                {
                    _game.CreateEntity()
                        .AddPlaceCardRequest(selectedCard.Id, slotId)
                        .With(x => x.isRequest = true)
                        ;
                    
                    Debug.Log($"[ProcessSlotClickToPlaceRequest] Created PlaceCardRequest card={selectedCard.Id} slot={slotId}");
                }

                request.Destroy();
            }
        }
    }
}