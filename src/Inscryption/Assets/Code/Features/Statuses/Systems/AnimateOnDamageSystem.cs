using System.Collections.Generic;
using Code.Features.Statuses.Services;
using Entitas;

namespace Code.Features.Statuses.Systems
{
    public class AnimateOnDamageSystem : ReactiveSystem<GameEntity>
    {
        public AnimateOnDamageSystem(GameContext context) : base(context)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Damaged.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasDamageAnimator;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
               entity.DamageAnimator.PlayDamageAnimation();
            }
        }
    }
}
