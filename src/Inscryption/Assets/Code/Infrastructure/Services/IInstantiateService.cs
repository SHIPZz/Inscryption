using UnityEngine;

namespace Code.Infrastructure.Services
{
	public interface IInstantiateService
	{
		GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation);
		T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Object;
	}
}

