using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Code.Infrastructure.Services
{
	public interface IAssetsService
	{
		UniTask InitializeAsync(CancellationToken cancellationToken = default);
		void CleanUp();
		void CleanUpCategory(string category);
		TAsset Load<TAsset>(string key) where TAsset : class;
		TComponent LoadPrefabWithComponent<TComponent>(string key) where TComponent : Component;
		UniTask<TAsset> LoadAsync<TAsset>(string key, string category = "common", CancellationToken cancellationToken = default) where TAsset : class;
		UniTask<TComponent> LoadAsyncForComponent<TComponent>(string key, string category = "common", CancellationToken cancellationToken = default) where TComponent : class;
		UniTask<TAsset> LoadAsync<TAsset>(AssetReference assetReference, string category = "common", CancellationToken cancellationToken = default) where TAsset : class;
		UniTask<TComponent> LoadPrefabWithComponentAsync<TComponent>(AssetReferenceT<GameObject> assetReference, string category = "common", CancellationToken cancellationToken = default) where TComponent : Component;
		UniTask<TComponent> LoadPrefabWithComponentAsync<TComponent>(string key, string category = "common", CancellationToken cancellationToken = default) where TComponent : Component;
		UniTask<TAsset[]> LoadAllAsync<TAsset>(List<string> keys) where TAsset : class;
		List<string> GetAssetsListByLabel<TAsset>(string label);
		List<string> GetAssetsListByLabel(string label, Type type = null);
		T[] GetAssetsByLabel<T>(string label) where T : class;
		UniTask<T[]> GetAssetsByLabelAsync<T>(string label, string category = "common", CancellationToken cancellationToken = default) where T : class;
		UniTask<List<string>> GetAssetsListByLabelAsync<TAsset>(string label);
		UniTask<List<string>> GetAssetsListByLabelAsync(string label, Type type = null, CancellationToken cancellationToken = default);
		GameObject LoadAssetFromResources(string path);
		T LoadAssetFromResources<T>(string path) where T : UnityEngine.Object;
	}
}
