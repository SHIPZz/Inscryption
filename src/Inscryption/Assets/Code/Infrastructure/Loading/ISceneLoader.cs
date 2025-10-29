using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Loading
{
    public interface ISceneLoader
    {
        UniTask LoadSceneAsync(string nextScene, CancellationToken cancellationToken = default);
    }
}