using System.Collections.Generic;
using Code.Common.Time;
using Entitas;
using UnityEngine;

namespace Code.Common.Systems
{
    public class ProcessTimerSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new(32);
        private readonly ITimeService _timeService;

        public ProcessTimerSystem(GameContext game, ITimeService timeService)
        {
            _timeService = timeService;
            _entities = game.GetGroup(GameMatcher.AllOf(
                GameMatcher.Timer)
                .NoneOf(GameMatcher.TimerEnded));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                float newTime = entity.Timer - _timeService.DeltaTime;

                if (newTime <= 0f)
                {
                    entity.isTimerEnded = true;
                    Debug.Log($"[ProcessTimerSystem] Timer ended for entity {entity.Id}");
                }
                else
                {
                    entity.ReplaceTimer(newTime);
                }
            }
        }
    }
}
