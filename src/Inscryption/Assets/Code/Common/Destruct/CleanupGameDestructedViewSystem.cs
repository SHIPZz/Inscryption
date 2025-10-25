using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Common.Systems.Destruct
{
    public class CleanupGameDestructedViewSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _group;
        private readonly List<GameEntity> _buffer = new(128);

        public CleanupGameDestructedViewSystem(GameContext game)
        {
            _group = game
                .GetGroup(GameMatcher
                    .AllOf(GameMatcher.Destructed, GameMatcher.View));
        }

        public void Cleanup()
        {
            foreach (GameEntity entity in _group.GetEntities(_buffer))
            {
                Object.Destroy(entity.View.gameObject);
                
                entity.View.ReleaseEntity();
            }
        }
    }
}