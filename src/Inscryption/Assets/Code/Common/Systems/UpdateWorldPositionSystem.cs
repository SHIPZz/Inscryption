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
                GameMatcher.Transform)
                .NoneOf(GameMatcher.Static,GameMatcher.LocalPosition));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                entity.Transform.position = entity.WorldPosition;
            }
        }
    }
}