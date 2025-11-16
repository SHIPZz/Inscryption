using System.Collections.Generic;
using Entitas;

namespace Code.Features.Cards.Systems
{
    public class UpdateSelectedCardVisualSystem : ReactiveSystem<GameEntity>
    {
        public UpdateSelectedCardVisualSystem(GameContext game) : base(game)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Selected.AddedOrRemoved());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isCard && entity.hasCardAnimator && entity.isSelectionAvailable;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (GameEntity card in entities)
            {
                if (card.isSelected)
                {
                    card.CardAnimator.Select();
                    continue;
                }

                card.CardAnimator.Deselect();
            }
        }
    }
}

