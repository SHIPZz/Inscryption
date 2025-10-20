using Entitas;

namespace Code.Common.Systems
{
    public class UpdateWorldRotationSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;

        public UpdateWorldRotationSystem(GameContext game)
        {
            _entities = game.GetGroup(GameMatcher.AllOf(
                GameMatcher.WorldRotation,
                GameMatcher.View));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                if (entity.View != null && entity.View.transform != null)
                {
                    entity.View.transform.rotation = entity.WorldRotation;
                }
            }
        }
    }
}

