using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Services
{
	public class InstantiateService : IInstantiateService
	{
		private readonly IInstantiator _instantiator;

		public InstantiateService(IInstantiator instantiator)
		{
			_instantiator = instantiator;
		}

		public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			return _instantiator.InstantiatePrefab(prefab, position, rotation, null);
		}

		public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Object
		{
			return _instantiator.InstantiatePrefabForComponent<T>(prefab, position, rotation, null);
		}
	}
}

