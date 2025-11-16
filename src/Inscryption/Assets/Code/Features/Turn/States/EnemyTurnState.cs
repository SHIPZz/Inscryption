using System;
using System.Threading;
using Code.Features.Turn;
using Code.Infrastructure.Systems;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;

namespace Code.Features.Turn.States
{
  public class EnemyTurnState : IState, IEnterState, IUpdateable, IExitableState, IDisposable
  {
    private readonly GameContext _game;
    private readonly ISystemFactory _systemFactory;

    private EnemyTurnFeature _enemyTurnFeature;

    public EnemyTurnState(GameContext game, ISystemFactory systemFactory)
    {
      _game = game;
      _systemFactory = systemFactory;
    }

    public async UniTask EnterAsync(CancellationToken cancellationToken = default)
    {
      _enemyTurnFeature = _systemFactory.Create<EnemyTurnFeature>();
      _enemyTurnFeature.Initialize();

      await UniTask.CompletedTask;
    }

    public void Update()
    {
      _enemyTurnFeature?.Execute();
      _enemyTurnFeature?.Cleanup();
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
      _enemyTurnFeature?.DeactivateReactiveSystems();
      _enemyTurnFeature?.TearDown();
      _enemyTurnFeature = null;
    }
  }
}
