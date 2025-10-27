using Code.Common.Extensions;
using Code.Common.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Camera.Systems
{
	public class AlignRotationTowardsCameraSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _entities;
		private readonly ICameraProvider _cameraProvider;

		public AlignRotationTowardsCameraSystem(GameContext game, ICameraProvider cameraProvider)
		{
			_entities = game.GetGroup(GameMatcher.AllOf(
				GameMatcher.TrackCameraRotation,
				GameMatcher.View,
				GameMatcher.Transform));
			_cameraProvider = cameraProvider;
		}

		public void Execute()
		{
			UnityEngine.Camera camera = _cameraProvider.MainCamera;

			if (camera == null)
				return;

			foreach (GameEntity entity in _entities)
			{
				if (entity.View == null || entity.Transform == null)
					continue;

				Vector3 position = entity.Transform.position;
				Quaternion rotation = position.GetLookRotationTo(camera.transform.position, ignoreY: false);
				entity.ReplaceWorldRotation(rotation);

				Debug.Log($"[AlignRotationTowardsCameraSystem] Set camera rotation for card {entity.Id} (hero: {entity.isHero}) to {rotation.eulerAngles}");
			}
		}
	}
}

