using Entitas;

namespace Code.Common.Systems
{
    public class UpdateLocalPositionSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;

        public UpdateLocalPositionSystem(GameContext game)
        {
            _entities = game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalPosition,
                GameMatcher.Transform)
                .NoneOf(GameMatcher.Static));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                entity.Transform.localPosition = entity.localPosition.Value;
            }
        }
    }
}
