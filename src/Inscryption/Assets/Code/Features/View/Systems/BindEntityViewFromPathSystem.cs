using System.Collections.Generic;
using Code.Features.View.Factory;
using Entitas;

namespace Code.Features.View.Systems
{
    public class BindEntityViewFromPathSystem : IExecuteSystem
    {
        private readonly IEntityViewFactory _entityViewFactory;
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new(32);

        public BindEntityViewFromPathSystem(Contexts contexts, IEntityViewFactory entityViewFactory)
        {
            _entityViewFactory = entityViewFactory;
            _entities = contexts.game.GetGroup(GameMatcher
                .AllOf(GameMatcher.ViewPath)
                .NoneOf(GameMatcher.View));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                _entityViewFactory.CreateViewFromResourcesPath(entity);
            }
        }
    }
}