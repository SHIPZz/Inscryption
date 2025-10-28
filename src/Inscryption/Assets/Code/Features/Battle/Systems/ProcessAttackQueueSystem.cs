using System.Collections.Generic;
using Code.Common.Time;
using Entitas;
using UnityEngine;

namespace Code.Features.Battle.Systems
{
    //todo refactor this
    public class ProcessAttackQueueSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly ITimeService _timeService;
        private readonly IGroup<GameEntity> _attackQueues;
        private readonly List<GameEntity> _buffer = new(2);

        public ProcessAttackQueueSystem(GameContext game, ITimeService timeService)
        {
            _game = game;
            _timeService = timeService;
            _attackQueues = game.GetGroup(GameMatcher.AllOf(GameMatcher.AttackQueue, GameMatcher.AttackQueueTimer));
        }

        public void Execute()
        {
            foreach (var queueEntity in _attackQueues.GetEntities(_buffer))
            {
                ProcessQueue(queueEntity);
            }
        }

        private void ProcessQueue(GameEntity queueEntity)
        {
            var timer = queueEntity.attackQueueTimer;
            var queue = queueEntity.attackQueue;

            if (timer.CurrentAttackIndex >= queue.Attacks.Count)
            {
                if (!timer.AllAttacksComplete)
                {
                    queueEntity.ReplaceAttackQueueTimer(
                        0f,
                        timer.DelayBetweenAttacks,
                        timer.CurrentAttackIndex,
                        timer.PostAttackDelay,
                        true
                    );
                    Debug.Log($"[ProcessAttackQueueSystem] All attacks complete, waiting {timer.PostAttackDelay}s before turn transition");
                    return;
                }

                timer.ElapsedTime += _timeService.DeltaTime;
                queueEntity.ReplaceAttackQueueTimer(
                    timer.ElapsedTime,
                    timer.DelayBetweenAttacks,
                    timer.CurrentAttackIndex,
                    timer.PostAttackDelay,
                    timer.AllAttacksComplete
                );

                if (timer.ElapsedTime >= timer.PostAttackDelay)
                {
                    Debug.Log("[ProcessAttackQueueSystem] Post-attack delay complete, triggering turn transition");
                    _game.CreateEntity().isSwitchTurnRequest = true;
                    queueEntity.isDestructed = true;
                }
                return;
            }

            timer.ElapsedTime += _timeService.DeltaTime;

            if (timer.ElapsedTime >= timer.DelayBetweenAttacks)
            {
                var attack = queue.Attacks[timer.CurrentAttackIndex];
                CreateAttackRequest(attack);

                timer.ElapsedTime = 0f;
                timer.CurrentAttackIndex++;

                queueEntity.ReplaceAttackQueueTimer(
                    timer.ElapsedTime,
                    timer.DelayBetweenAttacks,
                    timer.CurrentAttackIndex,
                    timer.PostAttackDelay,
                    timer.AllAttacksComplete
                );

                Debug.Log($"[ProcessAttackQueueSystem] Processing attack {timer.CurrentAttackIndex}/{queue.Attacks.Count}");
            }
        }

        private void CreateAttackRequest(Turn.QueuedAttack attack)
        {
            _game.CreateEntity().AddAttackRequest(attack.AttackerId, attack.TargetId, attack.Damage);

            GameEntity attacker = _game.GetEntityWithId(attack.AttackerId);
            GameEntity target = _game.GetEntityWithId(attack.TargetId);

            if (attacker != null && target != null)
            {
                string targetType = target.isCard ? $"card {target.Id}" : $"player {target.Id}";
                Debug.Log($"[ProcessAttackQueueSystem] Card {attack.AttackerId} attacks {targetType} for {attack.Damage} damage (Lane {attack.Lane})");
            }
        }
    }
}
