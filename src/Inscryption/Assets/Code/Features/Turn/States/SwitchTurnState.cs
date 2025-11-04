using System.Threading;
using Code.Features.Turn.Extensions;
using Code.Features.Turn.StateMachine;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
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
    private readonly GameConfig _gameConfig;

    public SwitchTurnState(GameContext game, IGameStateMachine gameStateMachine, IConfigService configService)
    {
      _game = game;
      _gameStateMachine = gameStateMachine;
      _heroes = game.GetGroup(GameMatcher.Hero);
      _enemies = game.GetGroup(GameMatcher.Enemy);
      _gameConfig = configService.GetConfig<GameConfig>();
    }

    public async UniTask EnterAsync(CancellationToken cancellationToken = default)
    {
      var (currentPlayer, nextPlayer) = TurnExtensions.GetBattleParticipants(_heroes, _enemies);

      if (currentPlayer == null || nextPlayer == null)
        return;

      if (currentPlayer.isHero)
        currentPlayer.isHeroTurn = false;
      else
        currentPlayer.isEnemyTurn = false;

      nextPlayer.ReplaceCardsPlacedThisTurn(0);

      if (nextPlayer.isHero)
      {
        float delay = _gameConfig.AnimationTiming.EnemyTurnDelay + _gameConfig.AnimationTiming.PostAttackDelay;
        await UniTask.Delay(System.TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
        _gameStateMachine.EnterAsync<HeroTurnState>(cancellationToken).Forget();
      }
      else if (nextPlayer.isEnemy)
      {
        _gameStateMachine.EnterAsync<EnemyTurnState>(cancellationToken).Forget();
      }
    }

    public async UniTask ExitAsync(CancellationToken cancellationToken = default)
    {
      await UniTask.CompletedTask;
    }
  }
}
