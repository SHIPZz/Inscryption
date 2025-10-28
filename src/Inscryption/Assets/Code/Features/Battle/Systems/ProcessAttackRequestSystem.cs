using System.Collections.Generic;
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
        private readonly List<GameEntity> _buffer = new(16);

        public ProcessAttackRequestSystem(GameContext game, IStatusFactory statusFactory)
        {
            _game = game;
            _statusFactory = statusFactory;

            _attackRequests = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.AttackRequest, GameMatcher.ProcessingAvailable));
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
                    request.isDestructed = true;
                    continue;
                }

                _statusFactory.CreateStatus(StatusTypeId.Damage, attackerId, targetId, damage);

                Debug.Log($"[ProcessAttackRequestSystem] Attack request: {attackerId} -> {targetId}, Damage: {damage}");

                request.isDestructed = true;
            }
        }
    }
}