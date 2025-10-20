using Code.Common.Collisions;
using Code.Features.View;
using UnityEngine;

namespace Code.Common.Physics
{
	public class RaycastService : IRaycastService
	{
		private readonly ICollisionRegistry _collisionRegistry;

		public RaycastService(ICollisionRegistry collisionRegistry)
		{
			_collisionRegistry = collisionRegistry;
		}

		public bool Raycast(Ray ray, out RaycastHit hit, float maxDistance = 100f, int layerMask = -1)
		{
			return UnityEngine.Physics.Raycast(ray, out hit, maxDistance, layerMask);
		}

		public GameEntity RaycastForEntity(Ray ray, float maxDistance = 100f, int layerMask = -1)
		{
			if (UnityEngine.Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
			{
				EntityBehaviour entityBehaviour = hit.collider.GetComponent<EntityBehaviour>();
				
				if (entityBehaviour != null && entityBehaviour.Entity is GameEntity gameEntity)
				{
					return gameEntity;
				}

				int instanceId = hit.collider.GetInstanceID();
				return _collisionRegistry.Get<GameEntity>(instanceId);
			}

			return null;
		}
	}
}

