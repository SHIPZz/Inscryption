using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Code.Features.UI.Services;
using Entitas;

namespace Code.Features.UI.Systems
{
    // todo refactor this
    public class UpdateHealthUISystem : IExecuteSystem
    {
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IUIProvider _uiProvider;

        public UpdateHealthUISystem(IHeroProvider heroProvider, IEnemyProvider enemyProvider, IUIProvider uiProvider)
        {
            _uiProvider = uiProvider;
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;
        }

        public void Execute()
        {
            GameHUD hud = _uiProvider.GameHUD;
            
            if (hud == null)
                return;

            GameEntity hero = _heroProvider.GetHero();
            GameEntity enemy = _enemyProvider.GetEnemy();

            hud.UpdateHeroHealth(hero?.Hp ?? 0,hero?.MaxHp ?? 0 );

            hud.UpdateEnemyHealth(enemy?.Hp ?? 0,enemy?.MaxHp ?? 0 );
        }
    }
}
