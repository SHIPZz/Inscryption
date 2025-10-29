using Code.Common;
using Entitas;
using UnityEngine;

namespace Code.Features.Input.Systems
{
    public class ProcessSlotClickToPlaceRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _requests;
        private readonly IGroup<GameEntity> _selectedCards;

        public ProcessSlotClickToPlaceRequestSystem(GameContext game)
        {
            _game = game;
            _requests = game.GetGroup(GameMatcher.SlotClickRequest);

            _selectedCards = game.GetGroup(GameMatcher.AllOf(GameMatcher.Card, GameMatcher.Selected));
        }

        public void Execute()
        {
            foreach (GameEntity request in _requests)
            foreach (GameEntity selectedCard in _selectedCards)
            {
                int slotId = request.SlotClickRequest;

                GameEntity slot = _game.GetEntityWithId(slotId);

                if (slot != null && slot.isBoardSlot)
                {
                    CreateEntity
                        .Request()
                        .AddPlaceCardRequest(selectedCard.Id, slotId)
                        ;
                    
                    Debug.Log($"[ProcessSlotClickToPlaceRequest] Created PlaceCardRequest card={selectedCard.Id} slot={slotId}");
                }
            }
        }
    }
}