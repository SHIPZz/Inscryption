using System;
using System.Threading;
using Code.Infrastructure.States.Factory;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Code.Infrastructure.States.StateMachine
{
    public class AppStateMachine : IStateMachine, ITickable, IDisposable
    {
        private readonly IStateFactory _stateFactory;
        
        private IState _activeState;
        private CancellationTokenSource _stateCancellationTokenSource;
        private IUpdateable _updateableState;

        public AppStateMachine(IStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
            _stateCancellationTokenSource = new CancellationTokenSource();
        }

        public void Tick()
        {
            _updateableState?.Update();
        }

        public async UniTask EnterAsync<TState>(CancellationToken cancellationToken = default)
            where TState : class, IState, IEnterState
        {
            if (_activeState != null && _activeState.GetType() == typeof(TState))
                return;

            IState state = await ChangeStateAsync<TState>(cancellationToken);

            IEnterState enterState = (IEnterState)state;

            if (_activeState is IUpdateable updateableState)
                _updateableState = updateableState;

            await enterState.EnterAsync(cancellationToken);
        }

        public async UniTask EnterAsync<TState, TPayload>(TPayload payload,
            CancellationToken cancellationToken = default) where TState : class, IState, IPayloadState<TPayload>
        {
            if (_activeState != null && _activeState.GetType() == typeof(TState))
                return;

            TState state = await ChangeStateAsync<TState>(cancellationToken);
            
            if (_activeState is IUpdateable updateableState)
                _updateableState = updateableState;
            
            await state.EnterAsync(payload, cancellationToken);
        }

        private async UniTask<TState> ChangeStateAsync<TState>(CancellationToken cancellationToken)
            where TState : class, IState
        {
            if (_activeState != null)
            {
                _stateCancellationTokenSource.Cancel();
                _stateCancellationTokenSource.Dispose();
                _stateCancellationTokenSource = new CancellationTokenSource();

                if (_activeState is IExitableState exitableState)
                    await exitableState.ExitAsync(cancellationToken);
            }

            TState state = _stateFactory.CreateState<TState>();
            _activeState = state;

            return state;
        }

        public void Dispose()
        {
            if(_activeState is IDisposable disposable)
                disposable.Dispose();
            
            _stateCancellationTokenSource?.Cancel();
            _stateCancellationTokenSource?.Dispose();
        }
    }
}