using System.Collections.Generic;
using Entitas;

namespace Code.Features.Death.Systems
{
    public class MarkDeadSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _targets;
        private readonly List<GameEntity> _buffer = new(32);

        public MarkDeadSystem(GameContext game)
        {
            _targets = game.GetGroup(GameMatcher.AllOf(GameMatcher.Hp).NoneOf(GameMatcher.Destructed));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _targets.GetEntities(_buffer))
            {
                if (entity.Hp <= 0)
                    entity.isDestructed = true;
            }
        }
    }
}
