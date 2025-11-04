using System.Threading;
using Code.Features.Turn.Extensions;
using Code.Features.Turn.StateMachine;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.States
{
  public class SwitchTurnState : IState, IEnterState, IExitableState
  {
    private readonly GameContext _game;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly IGroup<GameEntity> _heroes;
    private readonly IGroup<GameEntity> _enemies;

    public SwitchTurnState(GameContext game, IGameStateMachine gameStateMachine)
    {
      _game = game;
      _gameStateMachine = gameStateMachine;
      _heroes = game.GetGroup(GameMatcher.Hero);
      _enemies = game.GetGroup(GameMatcher.Enemy);
    }

    public async UniTask EnterAsync(CancellationToken cancellationToken = default)
    {
      Debug.Log("[SwitchTurnState] Switching turn");

      var (currentPlayer, nextPlayer) = TurnExtensions.GetBattleParticipants(_heroes, _enemies);

      if (currentPlayer == null || nextPlayer == null)
      {
        Debug.LogError($"[SwitchTurnState] Invalid participants: currentPlayer={currentPlayer?.Id.ToString() ?? "null"}, nextPlayer={nextPlayer?.Id.ToString() ?? "null"}");
        return;
      }

      if (currentPlayer.isHero)
        currentPlayer.isHeroTurn = false;
      else
        currentPlayer.isEnemyTurn = false;

      if (nextPlayer.isHero)
      {
        nextPlayer.isHeroTurn = true;
        nextPlayer.ReplaceCardsPlacedThisTurn(0);
        _gameStateMachine.EnterAsync<HeroTurnState>(cancellationToken).Forget();
      }
      else if (nextPlayer.isEnemy)
      {
        nextPlayer.isEnemyTurn = true;
        nextPlayer.ReplaceCardsPlacedThisTurn(0);
        _gameStateMachine.EnterAsync<EnemyTurnState>(cancellationToken).Forget();
      }
    }

    public async UniTask ExitAsync(CancellationToken cancellationToken = default)
    {
      Debug.Log("[SwitchTurnState] Exiting");
      await UniTask.CompletedTask;
    }
  }
}
