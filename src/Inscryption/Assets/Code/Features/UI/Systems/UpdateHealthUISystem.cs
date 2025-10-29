using Code.Features.UI.Services;
using Entitas;

namespace Code.Features.UI.Systems
{
    public class UpdateHealthUISystem : IExecuteSystem
    {
        private readonly IUIProvider _uiProvider;
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;

        public UpdateHealthUISystem(GameContext game, IUIProvider uiProvider)
        {
            _uiProvider = uiProvider;
           _heroes = game.GetGroup(GameMatcher.AllOf(GameMatcher.Hero, GameMatcher.Hp));
           _enemies = game.GetGroup(GameMatcher.AllOf(GameMatcher.Enemy, GameMatcher.Hp));
        }

        public void Execute()
        {
            GameHUD hud = _uiProvider.GameHUD;
            
            if (hud == null)
                return;

            foreach (GameEntity hero in _heroes)
            foreach (GameEntity enemy in _enemies)
            {
                hud.UpdateHeroHealth(hero?.Hp ?? 0,hero?.MaxHp ?? 0 );

                hud.UpdateEnemyHealth(enemy?.Hp ?? 0,enemy?.MaxHp ?? 0 );
            }
        }
    }
}
