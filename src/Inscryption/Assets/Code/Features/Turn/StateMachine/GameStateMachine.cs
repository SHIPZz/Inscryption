using System;
using System.Threading;
using Code.Features.Turn.States;
using Code.Infrastructure.States.Factory;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Code.Features.Turn.StateMachine
{
  public class GameStateMachine : IGameStateMachine
  {
    private readonly IStateFactory _stateFactory;

    private IState _activeState;
    private IUpdateable _updateableState;
    private CancellationTokenSource _stateCancellationTokenSource;

    public GameStateMachine(IStateFactory stateFactory)
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

    public async UniTask EnterAsync<TState, TPayload>(TPayload payload, CancellationToken cancellationToken = default)
      where TState : class, IState, IPayloadState<TPayload>
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

        if (_activeState is IDisposable disposableState)
          disposableState.Dispose();

        _activeState = null;
        _updateableState = null;
      }

      TState state = _stateFactory.CreateState<TState>();
      _activeState = state;

      return state;
    }

    public void Dispose()
    {
      if (_activeState is IDisposable disposable)
        disposable.Dispose();

      if (_stateCancellationTokenSource != null && !_stateCancellationTokenSource.IsCancellationRequested)
      {
        _stateCancellationTokenSource.Cancel();
      }

      _stateCancellationTokenSource?.Dispose();
      _stateCancellationTokenSource = null;
    }
  }
}
