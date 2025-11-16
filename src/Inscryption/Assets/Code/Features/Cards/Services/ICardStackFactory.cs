namespace Code.Features.Cards.Services
{
    public interface ICardStackFactory
    {
        GameEntity CreateCardStack(CardStackFactory.CardStackCreateData createData);
    }
}