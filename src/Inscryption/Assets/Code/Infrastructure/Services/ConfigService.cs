using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.Services
{
	public class ConfigService : IConfigService
	{
		private readonly IAssetsService _assetsService;
		private readonly Dictionary<string, ScriptableObject> _loadedConfigs = new();

		public ConfigService(IAssetsService assetsService)
		{
			_assetsService = assetsService;
		}

		public async UniTask Initialize(CancellationToken cancellationToken = default)
		{
			await UniTask.CompletedTask;
			Debug.Log($"{nameof(ConfigService)} initialized");
		}

		public T GetConfig<T>(string key) where T : ScriptableObject
		{
			if (_loadedConfigs.TryGetValue(key, out ScriptableObject config))
				return config as T;

			return null;
		}

		public T LoadConfig<T>(string addressableKey) where T : ScriptableObject
		{
			if (_loadedConfigs.TryGetValue(addressableKey, out ScriptableObject cached))
				return cached as T;

			T config = _assetsService.Load<T>(addressableKey);
			
			if (config != null)
				_loadedConfigs[addressableKey] = config;

			return config;
		}

		public async UniTask<T> LoadConfigAsync<T>(string addressableKey, CancellationToken cancellationToken = default) where T : ScriptableObject
		{
			if (_loadedConfigs.TryGetValue(addressableKey, out ScriptableObject cached))
				return cached as T;

			T config = await _assetsService.LoadAsync<T>(addressableKey, "configs", cancellationToken);
			
			if (config != null)
				_loadedConfigs[addressableKey] = config;

			return config;
		}

		public T[] GetConfigsByLabel<T>(string label) where T : ScriptableObject
		{
			return _assetsService.GetAssetsByLabel<T>(label);
		}

		public async UniTask<T[]> GetConfigsByLabelAsync<T>(string label, CancellationToken cancellationToken = default) where T : ScriptableObject
		{
			return await _assetsService.GetAssetsByLabelAsync<T>(label, "configs", cancellationToken);
		}
	}
}

