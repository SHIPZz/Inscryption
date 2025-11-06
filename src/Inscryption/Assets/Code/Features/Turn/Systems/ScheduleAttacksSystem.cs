using Code.Common;
using Code.Common.Time;
using Code.Features.Board.Extensions;
using Code.Features.Turn.Extensions;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class ScheduleAttacksSystem : IInitializeSystem
    {
        private readonly GameContext _game;
        private readonly ITimerService _timerService;
        private readonly GameConfig _gameConfig;
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _slots;

        public ScheduleAttacksSystem(
            GameContext game,
            ITimerService timerService,
            IConfigService configService)
        {
            _game = game;
            _timerService = timerService;
            _gameConfig = configService.GetConfig<GameConfig>();
            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
            _slots = game.GetGroup(GameMatcher.BoardSlot);
        }

        public void Initialize()
        {
            var (attacker, defender) = TurnExtensions.GetBattleParticipants(_heroes, _enemies);
            if (attacker == null)
                return;

            float delay = 0f;
            int attackCount = 0;

            foreach (var slot in _slots.GetOwnedSlots(attacker.Id))
            {
                if (!slot.TryGetOccupyingCard(out var attackerCard))
                    continue;

                var target = slot.FindOppositeTarget(defender);
                if (target == null)
                    continue;

                delay += _gameConfig.AnimationTiming.DelayBetweenAttacks;
                attackCount++;

                int attackerId = attackerCard.Id;
                int targetId = target.Id;
                int damage = attackerCard.Damage;
                float currentDelay = delay;

                _timerService.Schedule(currentDelay, () =>
                {
                    CreateEntity
                        .Request()
                        .AddAttackRequest(attackerId, targetId, damage);
                });
            }

            if (attackCount > 0)
            {
                float maxDelay = delay + _gameConfig.AnimationTiming.PostAttackDelay;
                _timerService.Schedule(maxDelay, () =>
                {
                    CreateEntity
                        .Request()
                        .isAllAttacksComplete = true;
                });
            }
        }
    }
}

