using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Features.Input.Systems
{
	public class ProcessSlotClickToPlaceRequestSystem : IExecuteSystem
	{
		private readonly InputContext _input;
		private readonly GameContext _game;
		private readonly IGroup<InputEntity> _requests;
		private readonly List<InputEntity> _buffer = new List<InputEntity>();

		public ProcessSlotClickToPlaceRequestSystem(InputContext input, GameContext game)
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
				if (_game.hasSelectedCards && _game.SelectedCards.Count > 0)
				{
					int selectedCardId = _game.SelectedCards[0];
					GameEntity card = _game.GetEntityWithId(selectedCardId);
					GameEntity slot = _game.GetEntityWithId(slotId);

					if (card != null && slot != null && slot.isBoardSlot)
					{
						_game.CreateEntity().AddPlaceCardRequest(selectedCardId, slotId);
						Debug.Log($"[ProcessSlotClickToPlaceRequest] Created PlaceCardRequest card={selectedCardId} slot={slotId}");
					}
				}

				request.Destroy();
			}
		}
	}
}


