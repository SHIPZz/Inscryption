using System.Threading;
using Code.Common.Extensions;
using Code.Features.Turn.StateMachine;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.States
{
  public class EnemyTurnState : IState, IEnterState, IExitableState
  {
    private readonly GameContext _game;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly IGroup<GameEntity> _enemies;

    public EnemyTurnState(GameContext game, IGameStateMachine gameStateMachine)
    {
      _game = game;
      _gameStateMachine = gameStateMachine;
      _enemies = game.GetGroup(GameMatcher.Enemy);
    }

    public async UniTask EnterAsync(CancellationToken cancellationToken = default)
    {
      Debug.Log("[EnemyTurnState] Enemy turn started");

      GameEntity enemy = _enemies.GetSingleEntity();

      if (enemy == null)
      {
        Debug.LogError("[EnemyTurnState] Enemy is null!");
        return;
      }

      Debug.Log($"[EnemyTurnState] Setting enemy {enemy.Id} isEnemyTurn = true (was {enemy.isEnemyTurn})");
      enemy.isEnemyTurn = true;
      _gameStateMachine.EnterAsync<DrawState, int>(enemy.Id, cancellationToken).Forget();
    }

    public async UniTask ExitAsync(CancellationToken cancellationToken = default)
    {
      Debug.Log("[EnemyTurnState] Exiting");
      await UniTask.CompletedTask;
    }
  }
}
