using Code.Common.Services;
using Entitas;

namespace Code.Features.Hero.Services
{
    public class HeroProvider : PlayerProvider, IHeroProvider
    {
        public HeroProvider(GameContext game) 
            : base(game, GameMatcher.Hero, GameMatcher.HeroTurn)
        {
        }

        public GameEntity GetHero() => GetPlayer();
        
        public GameEntity GetActiveHero() => GetActivePlayer();
        
        public bool IsHeroActive() => IsPlayerActive();
    }
}

