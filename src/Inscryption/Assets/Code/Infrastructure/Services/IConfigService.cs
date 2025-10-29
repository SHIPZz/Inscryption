using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.Services
{
	public interface IConfigService
	{
		UniTask InitializeAsync(CancellationToken cancellationToken = default);
		T GetConfig<T>(string key) where T : ScriptableObject;
		T LoadConfig<T>(string addressableKey) where T : ScriptableObject;
		UniTask<T> LoadConfigAsync<T>(string addressableKey, CancellationToken cancellationToken = default) where T : ScriptableObject;
		T[] GetConfigsByLabel<T>(string label) where T : ScriptableObject;
		UniTask<T[]> GetConfigsByLabelAsync<T>(string label, CancellationToken cancellationToken = default) where T : ScriptableObject;
		T GetConfig<T>() where T : ScriptableObject;
		UniTask<T> LoadConfigAsync<T>(CancellationToken cancellationToken = default) where T : ScriptableObject;
	}
}

