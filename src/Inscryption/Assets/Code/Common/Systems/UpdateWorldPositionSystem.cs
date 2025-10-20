using Entitas;

namespace Code.Common.Systems
{
    public class UpdateWorldPositionSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;

        public UpdateWorldPositionSystem(GameContext game)
        {
            _entities = game.GetGroup(GameMatcher.AllOf(
                GameMatcher.WorldPosition,
                GameMatcher.View));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                if (entity.View != null && entity.View.transform != null)
                {
                    entity.View.transform.position = entity.WorldPosition;
                }
            }
        }
    }
}

