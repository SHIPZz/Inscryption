using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.StateInfrastructure
{
    public interface IExitableState
    {
        UniTask ExitAsync(CancellationToken cancellationToken = default);
    }
}