using Code.Common.Collisions;
using UnityEngine;

namespace Code.Common.Physics
{
    public class RaycastService : IRaycastService
    {
        private readonly ICollisionRegistry _collisionRegistry;
        private readonly RaycastHit[] _results;

        public RaycastService(ICollisionRegistry collisionRegistry)
        {
            _collisionRegistry = collisionRegistry;
            _results = new RaycastHit[100];
        }

        public bool Raycast(Ray ray, out RaycastHit hit, float maxDistance = 100f, int layerMask = -1)
        {
            return UnityEngine.Physics.Raycast(ray, out hit, maxDistance, layerMask);
        }

        public GameEntity RaycastForEntity(Ray ray, float maxDistance = 100f, int layerMask = -1)
        {
            if (UnityEngine.Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
            {
                int instanceId = hit.collider.GetInstanceID();
                return _collisionRegistry.Get<GameEntity>(instanceId);
            }

            return null;
        }

        public GameEntity RaycastNonAllocForEntity(Ray ray, float maxDistance = 100f, int layerMask = -1)
        {
            ClearResults();

            int hitCount = UnityEngine.Physics.RaycastNonAlloc(ray, _results, maxDistance, layerMask);

            if (hitCount <= 0)
                return null;
            
            foreach (var hit in _results)
            {
                int instanceId = hit.collider.GetInstanceID();
                return _collisionRegistry.Get<GameEntity>(instanceId);
            }
            
            return null;
        }

        private void ClearResults()
        {
            for (int i = 0; i < _results.Length; i++)
            {
                _results[i] = default;
            }
        }
    }
}