using System.Threading;
using Code.Common;
using Code.Common.Extensions;
using Code.Common.Time;
using Code.Features.Board.Extensions;
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
  public class AttackState : IState, IPayloadState<int>, IExitableState
  {
    private readonly GameContext _game;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly ITimerService _timerService;
    private readonly GameConfig _gameConfig;
    private readonly IGroup<GameEntity> _heroes;
    private readonly IGroup<GameEntity> _enemies;
    private readonly IGroup<GameEntity> _slots;

    public AttackState(
      GameContext game,
      IGameStateMachine gameStateMachine,
      ITimerService timerService,
      IConfigService configService)
    {
      _game = game;
      _gameStateMachine = gameStateMachine;
      _timerService = timerService;
      _gameConfig = configService.GetConfig<GameConfig>();
      _heroes = game.GetGroup(GameMatcher.Hero);
      _enemies = game.GetGroup(GameMatcher.Enemy);
      _slots = game.GetGroup(GameMatcher.BoardSlot);
    }

    public async UniTask EnterAsync(int attackerId, CancellationToken cancellationToken = default)
    {
      var (attacker, defender) = TurnExtensions.GetBattleParticipants(_heroes, _enemies);

      float totalDelay = ScheduleAttacks(attacker, defender);

      if (totalDelay > 0)
      {
        await UniTask.Delay(System.TimeSpan.FromSeconds(totalDelay + _gameConfig.AnimationTiming.PostAttackDelay), cancellationToken: cancellationToken);
      }
      else
      {
        await UniTask.Delay(System.TimeSpan.FromSeconds(_gameConfig.AnimationTiming.PostAttackDelay), cancellationToken: cancellationToken);
      }

      _gameStateMachine.EnterAsync<SwitchTurnState>(cancellationToken).Forget();
    }

    private float ScheduleAttacks(GameEntity attacker, GameEntity defender)
    {
      float delay = 0f;

      foreach (GameEntity slot in _slots.GetOwnedSlots(attacker.Id))
      {
        if (!slot.TryGetOccupyingCard(out GameEntity attackerCard))
          continue;

        GameEntity target = slot.FindOppositeTarget(defender);

        if (target == null)
          continue;

        delay += _gameConfig.AnimationTiming.DelayBetweenAttacks;

        int attackerId = attackerCard.Id;
        int targetId = target.Id;
        int damage = attackerCard.Damage;
        float currentDelay = delay;

        _timerService.Schedule(currentDelay, () => {
          CreateEntity
            .Request()
            .AddAttackRequest(attackerId, targetId, damage);
        });
      }

      return delay;
    }

    public async UniTask ExitAsync(CancellationToken cancellationToken = default)
    {
      await UniTask.CompletedTask;
    }
  }
}
