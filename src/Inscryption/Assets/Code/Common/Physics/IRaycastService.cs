using UnityEngine;

namespace Code.Common.Physics
{
	public interface IRaycastService
	{
		bool Raycast(Ray ray, out RaycastHit hit, float maxDistance = 100f, int layerMask = -1);
		GameEntity RaycastForEntity(Ray ray, float maxDistance = 100f, int layerMask = -1);
		GameEntity RaycastNonAllocForEntity(Ray ray, float maxDistance = 100f, int layerMask = -1);
	}
}

