using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Cheats
{
    public class CheatService : ICheatService, ITickable
    {
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;

        public CheatService(IHeroProvider heroProvider, IEnemyProvider enemyProvider)
        {
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Debug.Log("[CheatService] F1 pressed - Killing Hero");
                KillHero();
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                Debug.Log("[CheatService] F2 pressed - Killing Enemy");
                KillEnemy();
            }
        }

        public void KillHero()
        {
            var hero = _heroProvider.GetHero();
            if (hero != null && !hero.isDestructed)
            {
                Debug.Log("[CheatService] Killing hero via cheat");
                hero.ReplaceHp(0);
                hero.isDestructed = true;
            }
        }

        public void KillEnemy()
        {
            var enemy = _enemyProvider.GetEnemy();
            if (enemy != null && !enemy.isDestructed)
            {
                Debug.Log("[CheatService] Killing enemy via cheat");
                enemy.ReplaceHp(0);
                enemy.isDestructed = true;
            }
        }
    }
}