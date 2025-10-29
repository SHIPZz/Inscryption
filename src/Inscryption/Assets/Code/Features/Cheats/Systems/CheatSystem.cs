using Code.Common.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Cheats.Systems
{
    public class CheatSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IInputService _inputService;

        public CheatSystem(GameContext game, IInputService inputService)
        {
            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
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
            foreach (var hero in _heroes)
            {
                if (!hero.isDestructed)
                {
                    Debug.Log("[CheatService] Killing hero via cheat");
                    hero.ReplaceHp(0);
                }
            }
        }

        public void KillEnemy()
        {
            foreach (var enemy in _enemies)
            {
                if (!enemy.isDestructed)
                {
                    Debug.Log("[CheatService] Killing enemy via cheat");
                    enemy.ReplaceHp(0);
                }
            }
        }
    }
}