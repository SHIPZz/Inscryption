using System.Collections.Generic;
using Code.Gameplay.Common.Time;
using Entitas;

namespace Code.Common.Systems.Destruct
{
    public class SelfDestructTimerSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new(128);
        private readonly ITimeService _timeService;

        public SelfDestructTimerSystem(GameContext game, ITimeService timeService)
        {
            _timeService = timeService;
            _entities = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.SelfDestructTimer)
                .NoneOf(GameMatcher.Destructed));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                if (entity.SelfDestructTimer > 0)
                {
                    entity.ReplaceSelfDestructTimer(entity.SelfDestructTimer - _timeService.DeltaTime);
                }
                else
                {
                    entity.RemoveSelfDestructTimer();
                    entity.isDestructed = true;
                }
            }
        }
    }
}