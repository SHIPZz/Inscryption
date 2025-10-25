using Entitas;

namespace Code.Common.Systems
{
    public class UpdateWorldLocalRotationSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;

        public UpdateWorldLocalRotationSystem(GameContext game)
        {
            _entities = game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalRotation,
                GameMatcher.Transform));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                entity.Transform.localRotation = entity.LocalRotation;
            }
        }
    }
}