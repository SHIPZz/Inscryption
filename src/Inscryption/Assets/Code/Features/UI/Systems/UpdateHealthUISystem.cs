using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Entitas;

namespace Code.Features.UI.Systems
{
    public class UpdateHealthUISystem : IExecuteSystem
    {
        private readonly MetaContext _meta;
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;

        public UpdateHealthUISystem(MetaContext meta, IHeroProvider heroProvider, IEnemyProvider enemyProvider)
        {
            _meta = meta;
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;
        }

        public void Execute()
        {
            if (!_meta.hasGameHUD)
                return;

            GameHUD hud = _meta.gameHUD.Value;
            
            if (hud == null)
                return;

            GameEntity hero = _heroProvider.GetHero();
            GameEntity enemy = _enemyProvider.GetEnemy();

            hud.UpdateHeroHealth(hero?.Hp ?? 0,hero?.MaxHp ?? 0 );

            hud.UpdateEnemyHealth(enemy?.Hp ?? 0,enemy?.MaxHp ?? 0 );
        }
    }
}
