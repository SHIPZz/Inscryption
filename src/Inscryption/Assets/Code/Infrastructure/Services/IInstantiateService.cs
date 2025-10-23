using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Services
{
	public interface IInstantiateService
	{
		GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation);
		T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Object;
		GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent);
		T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Object;
		T Instantiate<T>();
		void SetInstantiator(DiContainer diContainer);
	}
}

