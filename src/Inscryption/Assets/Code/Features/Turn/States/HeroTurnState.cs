using System;
using System.Threading;
using Code.Features.Turn;
using Code.Infrastructure.Systems;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;

namespace Code.Features.Turn.States
{
  public class HeroTurnState : IState, IEnterState, IUpdateable, IExitableState, IDisposable
  {
    private readonly GameContext _game;
    private readonly ISystemFactory _systemFactory;

    private HeroTurnFeature _heroTurnFeature;

    public HeroTurnState(GameContext game, ISystemFactory systemFactory)
    {
      _game = game;
      _systemFactory = systemFactory;
    }

    public async UniTask EnterAsync(CancellationToken cancellationToken = default)
    {
      _heroTurnFeature = _systemFactory.Create<HeroTurnFeature>();
      _heroTurnFeature.Initialize();

      await UniTask.CompletedTask;
    }

    public void Update()
    {
      _heroTurnFeature?.Execute();
      _heroTurnFeature?.Cleanup();
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
      _heroTurnFeature?.DeactivateReactiveSystems();
      _heroTurnFeature?.TearDown();
      _heroTurnFeature = null;
    }
  }
}
