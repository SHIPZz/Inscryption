using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Features.Stats.Systems
{
    public class ApplyHpFromStatsSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _targets;

        public ApplyHpFromStatsSystem(GameContext game)
        {
            _targets = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Stats, 
                    GameMatcher.Hp,
                    GameMatcher.StatsModifiers));
        }

        public void Execute()
        {
            foreach (GameEntity target in _targets)
            {
                if(target.StatsModifiers.Count <= 0)
                    continue;

                Dictionary<StatTypeId, int> stats = target.Stats;

                target.StatsModifiers.TryGetValue(StatTypeId.Hp, out int hpDelta);

                if (hpDelta == 0)
                    continue;

                stats.TryGetValue(StatTypeId.Hp, out int current);
                int newHp = Mathf.Max(0, current + hpDelta);
                stats[StatTypeId.Hp] = newHp;

                target.ReplaceHp(newHp);
                target.StatsModifiers[StatTypeId.Hp] = 0;

                string entityName = target.hasName ? target.Name : $"Entity {target.Id}";
                Debug.Log($"[ApplyHpFromStatsSystem] {entityName}: HP changed from {current} to {newHp} (delta: {hpDelta})");
            }
        }
    }
}


