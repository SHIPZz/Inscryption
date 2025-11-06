using System;
using System.Threading;
using Code.Features.Turn;
using Code.Infrastructure.Systems;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;

namespace Code.Features.Turn.States
{
  public class PlacementState : IState, IPayloadState<int>, IUpdateable, IExitableState, IDisposable
  {
    private readonly ISystemFactory _systemFactory;

    private PlacementFeature _placementFeature;

    public PlacementState(ISystemFactory systemFactory)
    {
      _systemFactory = systemFactory;
    }

    public async UniTask EnterAsync(int playerId, CancellationToken cancellationToken = default)
    {
      _placementFeature = _systemFactory.Create<PlacementFeature>();
      _placementFeature.Initialize();

      await UniTask.CompletedTask;
    }

    public void Update()
    {
      _placementFeature?.Execute();
      _placementFeature?.Cleanup();
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
      _placementFeature?.DeactivateReactiveSystems();
      _placementFeature?.TearDown();
      _placementFeature = null;
    }
  }
}
