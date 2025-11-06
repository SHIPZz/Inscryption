using System;
using System.Threading;
using Code.Features.Turn;
using Code.Infrastructure.Systems;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;

namespace Code.Features.Turn.States
{
  public class EnemyPlaceCardsState : IState, IPayloadState<int>, IUpdateable, IExitableState, IDisposable
  {
    private readonly GameContext _game;
    private readonly ISystemFactory _systemFactory;

    private EnemyPlaceCardsFeature _enemyPlaceCardsFeature;

    public EnemyPlaceCardsState(GameContext game, ISystemFactory systemFactory)
    {
      _game = game;
      _systemFactory = systemFactory;
    }

    public async UniTask EnterAsync(int enemyId, CancellationToken cancellationToken = default)
    {
      GameEntity enemy = _game.GetEntityWithId(enemyId);
      if (enemy == null)
      {
        UnityEngine.Debug.LogWarning($"[EnemyPlaceCardsState] Enemy with id {enemyId} not found");
        return;
      }

      UnityEngine.Debug.Log($"[EnemyPlaceCardsState] Entering for enemy {enemyId}, isEnemyTurn: {enemy.isEnemyTurn}, cardsInHand: {enemy.CardsInHand.Count}");

      _enemyPlaceCardsFeature = _systemFactory.Create<EnemyPlaceCardsFeature>();
      _enemyPlaceCardsFeature.Initialize();

      await UniTask.CompletedTask;
    }

    public void Update()
    {
      _enemyPlaceCardsFeature?.Execute();
      _enemyPlaceCardsFeature?.Cleanup();
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
      _enemyPlaceCardsFeature?.DeactivateReactiveSystems();
      _enemyPlaceCardsFeature?.TearDown();
      _enemyPlaceCardsFeature = null;
    }
  }
}