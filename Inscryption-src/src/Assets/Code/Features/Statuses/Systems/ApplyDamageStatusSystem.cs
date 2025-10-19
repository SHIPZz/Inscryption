using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Features.Statuses.Systems
{
    public class ApplyDamageStatusSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _damageStatuses;
        private readonly List<GameEntity> _buffer = new(32);

        public ApplyDamageStatusSystem(GameContext game)
        {
            _game = game;
            _damageStatuses = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.DamageStatus,
                    GameMatcher.StatusTarget)
                .NoneOf(GameMatcher.Destructed));
            
            Debug.Log("[ApplyDamageStatusSystem] System initialized");
        }

        public void Execute()
        {
            foreach (GameEntity status in _damageStatuses.GetEntities(_buffer))
            {
                Debug.Log($"[ApplyDamageStatusSystem] Processing damage status ID: {status.Id}");
                
                if (!status.hasStatusTarget)
                {
                    Debug.LogWarning($"[ApplyDamageStatusSystem] Status {status.Id} has no target!");
                    continue;
                }

                int targetId = status.StatusTarget;
                GameEntity target = _game.GetEntityWithId(targetId);
                
                if (target == null)
                {
                    Debug.LogError($"[ApplyDamageStatusSystem] Target entity {targetId} not found!");
                    status.isDestructed = true;
                    continue;
                }
                
                if (!target.hasHp)
                {
                    Debug.LogWarning($"[ApplyDamageStatusSystem] Target {targetId} has no HP component!");
                    status.isDestructed = true;
                    continue;
                }

                int oldHp = target.Hp;
                target.ReplaceHp(oldHp - 1);
                Debug.Log($"[ApplyDamageStatusSystem] Applied damage to target {targetId}: {oldHp} -> {target.Hp}");
                
                if (target.Hp <= 0)
                {
                    target.isDestructed = true;
                    Debug.Log($"[ApplyDamageStatusSystem] Target {targetId} HP reached 0, marked as Destructed");
                }

                status.isDestructed = true;
                Debug.Log($"[ApplyDamageStatusSystem] Status {status.Id} marked as Destructed");
            }
        }
    }
}


