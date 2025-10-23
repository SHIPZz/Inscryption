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
            if (hero != null && hero.hasHp && hero.hasMaxHp)
            {
                hud.UpdateHeroHealth(hero.Hp, hero.MaxHp);
            }

            GameEntity enemy = _enemyProvider.GetEnemy();
            if (enemy != null && enemy.hasHp && enemy.hasMaxHp)
            {
                hud.UpdateEnemyHealth(enemy.Hp, enemy.MaxHp);
            }
        }
    }
}
