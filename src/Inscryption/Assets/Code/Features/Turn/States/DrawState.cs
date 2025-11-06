using System;
using System.Threading;
using Code.Features.Turn;
using Code.Infrastructure.Systems;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;

namespace Code.Features.Turn.States
{
  public class DrawState : IState, IPayloadState<int>, IUpdateable, IExitableState, IDisposable
  {
    private readonly ISystemFactory _systemFactory;

    private DrawFeature _drawFeature;

    public DrawState(ISystemFactory systemFactory)
    {
      _systemFactory = systemFactory;
    }

    public async UniTask EnterAsync(int playerId, CancellationToken cancellationToken = default)
    {
      _drawFeature = _systemFactory.Create<DrawFeature>();
      _drawFeature.Initialize();

      await UniTask.CompletedTask;
    }

    public void Update()
    {
      _drawFeature?.Execute();
      _drawFeature?.Cleanup();
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
      _drawFeature?.DeactivateReactiveSystems();
      _drawFeature?.TearDown();
      _drawFeature = null;
    }
  }
}
