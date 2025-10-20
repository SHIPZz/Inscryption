namespace Code.Features.Hero.Services
{
    public interface IHeroFactory
    {
        GameEntity CreateHero(int baseHealth);
    }
}

