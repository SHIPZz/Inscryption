using System.Collections.Generic;
using Code.Features.UI.Services;
using Entitas;

namespace Code.Features.UI.Systems
{
    public class UpdateTurnIndicatorUISystem : ReactiveSystem<GameEntity>
    {
        private readonly IUIProvider _uiProvider;

        public UpdateTurnIndicatorUISystem(GameContext game, IUIProvider uiProvider) : base(game)
        {
            _uiProvider = uiProvider;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.HeroTurn.Added(), GameMatcher.EnemyTurn.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return (entity.isHero || entity.isEnemy) && !entity.isDestructed;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.isHero && entity.isHeroTurn)
                {
                    _uiProvider.GameHUD.SetHeroTurn(true);
                }
                else if (entity.isEnemy && entity.isEnemyTurn)
                {
                    _uiProvider.GameHUD.SetHeroTurn(false);
                }
            }
        }
    }
}