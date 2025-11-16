using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.Loading
{
    public class SceneLoadService : ISceneLoader
    {
        public async UniTask LoadSceneAsync(string nextScene, CancellationToken cancellationToken = default)
        {
            if (IsSceneLoaded(nextScene))
            {
                Debug.Log($"[SceneLoadService] Scene {nextScene} is already loaded, skipping load");
                return;
            }

            Debug.Log($"[SceneLoadService] Loading scene: {nextScene}");
            await Addressables.LoadSceneAsync(nextScene).ToUniTask(cancellationToken: cancellationToken);
            await UniTask.NextFrame(cancellationToken);
        }

        private bool IsSceneLoaded(string sceneName)
        {
            return SceneManager.GetActiveScene().name == sceneName;
        }
    }
}