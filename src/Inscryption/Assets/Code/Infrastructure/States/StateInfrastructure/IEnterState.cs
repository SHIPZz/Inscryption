using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.StateInfrastructure
{
    public interface IEnterState : IState
    {
        UniTask EnterAsync(CancellationToken cancellationToken = default);
    }
}