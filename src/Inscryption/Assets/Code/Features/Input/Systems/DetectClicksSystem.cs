using Code.Common;
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
        private readonly IGroup<InputEntity> _inputGroup;

        public DetectClicksSystem(InputContext input, ICameraProvider cameraProvider, IRaycastService raycastService,
            IInputService inputService)
        {
            _input = input;
            _cameraProvider = cameraProvider;
            _raycastService = raycastService;
            _inputService = inputService;

            _inputGroup = _input.GetGroup(InputMatcher.AllOf(InputMatcher.Input, InputMatcher.LayerMask));
        }

        public void Execute()
        {
            foreach (InputEntity input in _inputGroup)
            {
                if (_inputService.GetMouseButtonDown(0))
                {
                    HandleMouseClick(input);
                }
            }
        }

        private void HandleMouseClick(InputEntity input)
        {
            UnityEngine.Camera camera = _cameraProvider.MainCamera;

            if (camera == null)
                return;

            Ray ray = camera.ScreenPointToRay(_inputService.MousePosition);
            
            GameEntity entity = _raycastService.RaycastNonAllocForEntity(ray, 100f, input.LayerMask);

            if (entity == null)
                return;

            if (entity.isCard && entity.isSelectionAvailable)
            {
                CreateEntity
                    .Request()
                    .AddCardClickRequest(entity.Id);
            }
            else if (entity.isBoardSlot)
            {
                CreateEntity
                    .Request()
                    .AddSlotClickRequest(entity.Id);
            }
        }
    }
}