using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Features.View.Factory
{
	public interface IViewFactory
	{
		IUnityView CreateViewFromResourcesPath(string path, Vector3 at);
		IUnityView CreateViewFromPrefab(IUnityView prefab, Vector3 at);
		UniTask<IUnityView> CreateViewFromAssetReference(AssetReference assetReference, Vector3 at, CancellationToken cancellationToken = default);
		UniTask<IUnityView> CreateViewFromAddressableKey(string addressableKey, Vector3 at, CancellationToken cancellationToken = default);
	}
}