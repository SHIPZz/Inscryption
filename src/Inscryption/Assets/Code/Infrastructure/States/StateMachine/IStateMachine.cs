using System.Threading;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.StateMachine
{
    public interface IStateMachine
    {
        UniTask EnterAsync<TState>(CancellationToken cancellationToken = default) where TState : class, IState, IEnterState;
        UniTask EnterAsync<TState, TPayload>(TPayload payload, CancellationToken cancellationToken = default) where TState : class, IState, IPayloadState<TPayload>;
    }
}