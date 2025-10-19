namespace Code.Features.Cards.Services
{
    public interface ICardFactory
    {
        GameEntity CreateCard(int hp, int damage, int ownerId);
        GameEntity CreateRandomCard(int ownerId);
    }
}

