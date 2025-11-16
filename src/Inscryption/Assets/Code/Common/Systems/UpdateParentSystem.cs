using Code.Common.Extensions;
using Entitas;

namespace Code.Common.Systems
{
    public class UpdateParentSystem : ReactiveSystem<GameEntity>
    {
        public UpdateParentSystem(GameContext context) : base(context)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Parent, GameMatcher.Transform));
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasParent && entity.hasTransform;
        }

        protected override void Execute(System.Collections.Generic.List<GameEntity> entities)
        {
            foreach (GameEntity entity in entities)
            {
                bool worldPositionStays = !entity.hasLocalPosition;
                entity.SetParent(entity.parent.Value, worldPositionStays);
            }
        }
    }
}

