using Code.Common;
using Code.Common.Extensions;
using Code.Common.Time;
using Code.Features.Board.Extensions;
using Code.Features.Turn.Extensions;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Battle.Systems
{
    public class CreateAttacksOnEndTurnSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _endTurnRequests;
        private readonly IGroup<GameEntity> _slots;
        private readonly GameConfig _gameConfig;
        private readonly ITimerService _timerService;

        public CreateAttacksOnEndTurnSystem(GameContext game, IConfigService configService, ITimerService timerService)
        {
            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
            _endTurnRequests = game.GetGroup(GameMatcher.EndTurnRequest);
            _slots = game.GetGroup(GameMatcher.BoardSlot);
            _gameConfig = configService.GetConfig<GameConfig>();
            _timerService = timerService;
        }

        public void Execute()
        {
            foreach (GameEntity request in _endTurnRequests)
            {
                var (attacker, defender) = TurnExtensions.GetBattleParticipants(_heroes, _enemies);

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

                _timerService.Schedule(currentDelay, () =>
                    CreateEntity
                        .Request()
                        .AddAttackRequest(attackerId, targetId, damage));
            }

            float switchTurnDelay = delay + _gameConfig.AnimationTiming.PostAttackDelay;

            _timerService.Schedule(switchTurnDelay, () =>
                CreateEntity
                    .Request()
                    .With(x => x.isSwitchTurnRequest = true));
        }
    }
}