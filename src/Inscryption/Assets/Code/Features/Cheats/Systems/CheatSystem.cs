using Code.Common.Services;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Cheats.Systems
{
    public class CheatSystem : IExecuteSystem
    {
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IInputService _inputService;

        public CheatSystem(IHeroProvider heroProvider, IEnemyProvider enemyProvider, IInputService inputService)
        {
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;
            _inputService = inputService;
        }

        public void Execute()
        {
            if (_inputService.GetKeyDown(KeyCode.F1))
            {
                Debug.Log("[CheatService] F1 pressed - Killing Hero");
                KillHero();
            }

            if (_inputService.GetKeyDown(KeyCode.F2))
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
            }
        }

        public void KillEnemy()
        {
            var enemy = _enemyProvider.GetEnemy();
            if (enemy != null && !enemy.isDestructed)
            {
                Debug.Log("[CheatService] Killing enemy via cheat");
                enemy.ReplaceHp(0);
            }
        }
    }
}