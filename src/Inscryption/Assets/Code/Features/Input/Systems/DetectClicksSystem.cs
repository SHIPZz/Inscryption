using Code.Common.Physics;
using Code.Common.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Input.Systems
{
	public class DetectClicksSystem : IExecuteSystem
	{
		private readonly InputContext _input;
		private readonly ICameraProvider _cameraProvider;
		private readonly IRaycastService _raycastService;
		private readonly IInputService _inputService;

		public DetectClicksSystem(InputContext input, ICameraProvider cameraProvider, IRaycastService raycastService, IInputService inputService)
		{
			_input = input;
			_cameraProvider = cameraProvider;
			_raycastService = raycastService;
			_inputService = inputService;
		}

		public void Execute()
		{
			if (_inputService.GetMouseButtonDown(0))
			{
				HandleMouseClick();
			}
		}

		private void HandleMouseClick()
		{
			UnityEngine.Camera camera = _cameraProvider.MainCamera;
			
			if (camera == null)
				return;

			Ray ray = camera.ScreenPointToRay(_inputService.MousePosition);
			GameEntity entity = _raycastService.RaycastForEntity(ray);

			if (entity != null)
			{
				if (entity.isCard)
				{
					_input.CreateEntity()
						.AddCardClickRequest(entity.Id);
				}
				else if (entity.isBoardSlot)
				{
					_input.CreateEntity()
						.AddSlotClickRequest(entity.Id);
				}
			}
		}
	}
}

