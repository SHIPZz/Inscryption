using System;
using System.Threading;
using Code.Features.Turn;
using Code.Infrastructure.Systems;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;

namespace Code.Features.Turn.States
{
  public class AttackState : IState, IPayloadState<int>, IUpdateable, IExitableState, IDisposable
  {
    private readonly ISystemFactory _systemFactory;

    private AttackFeature _attackFeature;

    public AttackState(ISystemFactory systemFactory)
    {
      _systemFactory = systemFactory;
    }

    public async UniTask EnterAsync(int attackerId, CancellationToken cancellationToken = default)
    {
      _attackFeature = _systemFactory.Create<AttackFeature>();
      _attackFeature.Initialize();

      await UniTask.CompletedTask;
    }

    public void Update()
    {
      _attackFeature?.Execute();
      _attackFeature?.Cleanup();
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
      _attackFeature?.DeactivateReactiveSystems();
      _attackFeature?.TearDown();
      _attackFeature = null;
    }
  }
}
