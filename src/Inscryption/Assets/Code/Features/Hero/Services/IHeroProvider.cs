using Entitas;

namespace Code.Features.Hero.Services
{
    public interface IHeroProvider
    {
        GameEntity GetHero();
        GameEntity GetActiveHero();
        bool IsHeroActive();
    }
}

