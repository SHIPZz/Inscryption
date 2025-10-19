using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Features.Battle.Systems
{
    public class ProcessAttackRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _attackRequests;
        private readonly List<GameEntity> _buffer = new(16);

        public ProcessAttackRequestSystem(GameContext game)
        {
            _game = game;

            _attackRequests = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.AttackRequest));
        }

        public void Execute()
        {
            foreach (GameEntity request in _attackRequests.GetEntities(_buffer))
            {
                int attackerId = request.attackRequest.AttackerId;
                int targetId = request.attackRequest.TargetId;
                int damage = request.attackRequest.Damage;

                GameEntity attacker = _game.GetEntityWithId(attackerId);
                GameEntity target = _game.GetEntityWithId(targetId);

                if (attacker == null || target == null)
                {
                    Debug.LogWarning($"[ProcessAttackRequestSystem] Invalid attack: attacker={attackerId}, target={targetId}");
                    request.isDestructed = true;
                    continue;
                }

                if (!target.hasHp)
                {
                    Debug.LogWarning($"[ProcessAttackRequestSystem] Target {targetId} has no HP!");
                    request.isDestructed = true;
                    continue;
                }

                int oldHp = target.Hp;
                target.ReplaceHp(oldHp - damage);

                Debug.Log($"[ProcessAttackRequestSystem] Attack: {attackerId} -> {targetId}, Damage: {damage}, HP: {oldHp} -> {target.Hp}");

                if (target.Hp <= 0)
                {
                    target.isDestructed = true;
                    Debug.Log($"[ProcessAttackRequestSystem] Target {targetId} destroyed!");

                    if (target.isCard && target.isOnBoard)
                    {
                        FreeSlot(target);
                    }
                }

                request.isDestructed = true;
            }
        }

        private void FreeSlot(GameEntity card)
        {
            if (!card.hasLane)
                return;

            foreach (GameEntity slot in _game.GetEntities(GameMatcher.BoardSlot))
            {
                if (slot.OccupiedBy == card.Id)
                {
                    slot.ReplaceOccupiedBy(-1);
                    Debug.Log($"[ProcessAttackRequestSystem] Slot {slot.Id} freed (lane {slot.SlotLane})");
                    break;
                }
            }
        }
    }
}

