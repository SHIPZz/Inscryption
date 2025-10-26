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
                GameMatcher.Transform)
                .NoneOf(GameMatcher.LocalRotation,GameMatcher.Static));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                entity.Transform.rotation = entity.WorldRotation;
            }
        }
    }
}