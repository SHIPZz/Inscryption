using Code.Common.Extensions;
using Code.Features.Battle.Services;
using Code.Features.Board.Extensions;
using Code.Features.Turn.Extensions;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Battle.Systems
{
    public class CreateAttacksOnEndTurnSystem_Refactored : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _endTurnRequests;
        private readonly IGroup<GameEntity> _slots;
        private readonly GameConfig _gameConfig;
        private readonly IAttackSchedulerService _attackScheduler;

        public CreateAttacksOnEndTurnSystem_Refactored(
            GameContext game,
            IConfigService configService,
            IAttackSchedulerService attackScheduler)
        {
            _game = game;
            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
            _endTurnRequests = game.GetGroup(GameMatcher.EndTurnRequest);
            _slots = game.GetGroup(GameMatcher.BoardSlot);
            _gameConfig = configService.GetConfig<GameConfig>();
            _attackScheduler = attackScheduler;
        }

        public void Execute()
        {
            foreach (GameEntity request in _endTurnRequests)
            {
                var (attacker, defender) = _game.GetBattleParticipants(_heroes, _enemies);

                if (attacker != null)
                {
                    ProcessAttacks(attacker, defender);
                }
            }
        }

        private void ProcessAttacks(GameEntity attacker, GameEntity defender)
        {
            string attackerName = attacker.isHero ? "Hero" : "Enemy";
            Debug.Log($"[CreateAttacksOnEndTurnSystem] Creating attack queue for {attackerName}'s cards");

            float delay = 0f;

            foreach (GameEntity slot in _slots.GetOwnedSlots(attacker.Id))
            {
                if (!slot.TryGetOccupyingCard(_game, out GameEntity attackerCard))
                    continue;

                GameEntity target = slot.FindOppositeTarget(_game, defender);

                if (target != null)
                {
                    delay += _gameConfig.AnimationTiming.DelayBetweenAttacks;
                    _attackScheduler.ScheduleAttack(delay, attackerCard.Id, target.Id, attackerCard.Damage);
                }
            }

            float switchTurnDelay = delay + _gameConfig.AnimationTiming.PostAttackDelay;
            _attackScheduler.ScheduleSwitchTurn(switchTurnDelay);
        }
    }
}
