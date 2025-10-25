using System.Collections.Generic;
using Code.Features.Stats;
using Entitas;
using UnityEngine;

namespace Code.Features.Statuses.Systems
{
    public class ApplyHpFromStatsSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _targets;
        private readonly List<GameEntity> _buffer = new(64);

        public ApplyHpFromStatsSystem(GameContext game)
        {
            _targets = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Stats, 
                    GameMatcher.Hp,
                    GameMatcher.StatsModifiers));
        }

        public void Execute()
        {
            foreach (GameEntity target in _targets.GetEntities(_buffer))
            {
                if(target.StatsModifiers.Count <= 0)
                    return;
                
                Dictionary<StatTypeId, int> stats = target.Stats;
                
                target.StatsModifiers.TryGetValue(StatTypeId.Hp, out int hpDelta);
                
                stats.TryGetValue(StatTypeId.Hp, out int current);
                stats[StatTypeId.Hp] = current + hpDelta;
                
                target.ReplaceHp(stats[StatTypeId.Hp]);
                target.StatsModifiers[StatTypeId.Hp] = 0;
            }
        }
    }
}


