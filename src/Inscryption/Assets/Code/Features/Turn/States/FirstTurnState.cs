using System;
using System.Threading;
using Code.Features.Turn;
using Code.Infrastructure.Systems;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;

namespace Code.Features.Turn.States
{
  public class FirstTurnState : IEnterState, IExitableState, IDisposable
  {
    private readonly ISystemFactory _systemFactory;

    private FirstTurnFeature _firstTurnFeature;

    public FirstTurnState(ISystemFactory systemFactory)
    {
      _systemFactory = systemFactory;
    }

    public async UniTask EnterAsync(CancellationToken cancellationToken = default)
    {
      _firstTurnFeature = _systemFactory.Create<FirstTurnFeature>();
      _firstTurnFeature.Initialize();

      await UniTask.CompletedTask;
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
      _firstTurnFeature?.DeactivateReactiveSystems();
      _firstTurnFeature?.TearDown();
      _firstTurnFeature = null;
    }
  }
}
