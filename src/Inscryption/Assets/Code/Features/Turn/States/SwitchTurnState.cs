using System;
using System.Threading;
using Code.Features.Turn;
using Code.Infrastructure.Systems;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;

namespace Code.Features.Turn.States
{
  public class SwitchTurnState : IState, IEnterState, IUpdateable, IExitableState, IDisposable
  {
    private readonly GameContext _game;
    private readonly ISystemFactory _systemFactory;

    private SwitchTurnFeature _switchTurnFeature;

    public SwitchTurnState(GameContext game, ISystemFactory systemFactory)
    {
      _game = game;
      _systemFactory = systemFactory;
    }

    public async UniTask EnterAsync(CancellationToken cancellationToken = default)
    {
      _switchTurnFeature = _systemFactory.Create<SwitchTurnFeature>();
      _switchTurnFeature.Initialize();

      await UniTask.CompletedTask;
    }

    public void Update()
    {
      _switchTurnFeature?.Execute();
      _switchTurnFeature?.Cleanup();
    }

    public async UniTask ExitAsync(CancellationToken cancellationToken = default)
    {
      Cleanup();
      await UniTask.CompletedTask;
    }

    public void Dispose()
    {
      Cleanup();
    }

    private void Cleanup()
    {
      _switchTurnFeature?.DeactivateReactiveSystems();
      _switchTurnFeature?.TearDown();
      _switchTurnFeature = null;
    }
  }
}
