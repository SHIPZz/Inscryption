using System.Linq;
using Code.Features.Board;
using Code.Features.Statuses.Components;
using Code.Features.Statuses.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Battle.Systems
{
    public class ProcessAttackRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IStatusFactory _statusFactory;
        private readonly IGroup<GameEntity> _attackRequests;
        private readonly IGroup<GameEntity> _slots;
        private readonly System.Collections.Generic.List<GameEntity> _buffer = new(32);

        public ProcessAttackRequestSystem(GameContext game, IStatusFactory statusFactory)
        {
            _game = game;
            _statusFactory = statusFactory;

            _attackRequests = game.GetGroup(GameMatcher.AttackRequest);
            _slots = game.GetGroup(GameMatcher.BoardSlot);
        }

        public void Execute()
        {
            foreach (GameEntity request in _attackRequests.GetEntities(_buffer))
            {
                Debug.Log("@@@ request created");
                int attackerId = request.attackRequest.AttackerId;
                int targetId = request.attackRequest.TargetId;
                int damage = request.attackRequest.Damage;

                GameEntity attacker = _game.GetEntityWithId(attackerId);
                GameEntity target = _game.GetEntityWithId(targetId);

                if (attacker == null || target == null)
                {
                    Debug.LogWarning($"[ProcessAttackRequestSystem] Invalid attack: attacker={attackerId}, target={targetId}");
                    request.Destroy();
                    continue;
                }

                if (attacker.hasAttackAnimator && TryGetTargetTransform(attacker, target, out Transform targetTransform))
                {
                    attacker.AttackAnimator.PlayAttackAnimation(targetTransform);
                }

                _statusFactory.CreateStatus(StatusTypeId.Damage, attackerId, targetId, damage);

                Debug.Log($"[ProcessAttackRequestSystem] Attack request: {attackerId} -> {targetId}, Damage: {damage}");

                request.Destroy();
            }
        }

        private bool TryGetTargetTransform(GameEntity attacker, GameEntity target, out Transform targetTransform)
        {
            targetTransform = null;

            if (target.hasView)
            {
                targetTransform = target.Transform;
                return targetTransform != null;
            }

            GameEntity attackerSlot = _slots.GetEntities()
                .FirstOrDefault(s => s.isOccupied && s.OccupiedBy == attacker.Id);

            if (attackerSlot == null)
                return false;

            GameEntity oppositeSlot = BoardHelpers.FindOppositeSlot(_game, attackerSlot);

            if (oppositeSlot != null && oppositeSlot.hasView)
            {
                targetTransform = oppositeSlot.Transform;
                return targetTransform != null;
            }

            return false;
        }
    }
}