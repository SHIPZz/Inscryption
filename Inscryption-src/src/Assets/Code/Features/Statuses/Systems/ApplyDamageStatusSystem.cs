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
                    GameMatcher.StatusTarget,
                    GameMatcher.StatusValue)
                .NoneOf(GameMatcher.Destructed));
        }

        public void Execute()
        {
            foreach (GameEntity status in _damageStatuses.GetEntities(_buffer))
            {
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
                    Debug.LogWarning($"[ApplyDamageStatusSystem] Target {targetId} has no HP!");
                    status.isDestructed = true;
                    continue;
                }

                int damageValue = status.StatusValue;
                int oldHp = target.Hp;
                target.ReplaceHp(oldHp - damageValue);
                
                Debug.Log($"[ApplyDamageStatusSystem] Applied {damageValue} damage: {oldHp} -> {target.Hp}");
                
                if (target.Hp <= 0)
                {
                    target.isDestructed = true;
                }

                status.isDestructed = true;
            }
        }
    }
}


