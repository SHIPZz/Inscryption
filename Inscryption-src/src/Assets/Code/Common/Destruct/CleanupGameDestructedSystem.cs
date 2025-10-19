using System.Collections.Generic;
using Entitas;

namespace Code.Common.Systems.Destruct
{
    public class CleanupGameDestructedSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _group;
        private readonly List<GameEntity> _buffer = new(128);

        public CleanupGameDestructedSystem(GameContext game)
        {
            _group = game.GetGroup(GameMatcher.Destructed);
        }

        public void Cleanup()
        {
            foreach (GameEntity entity in _group.GetEntities(_buffer))
            {
                entity.Destroy();
            }
        }
    }
}