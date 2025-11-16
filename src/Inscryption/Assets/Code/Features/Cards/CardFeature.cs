using Code.Features.Board.Systems;
using Code.Features.Cards.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Cards
{
    public class CardFeature : Feature
    {
        public CardFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<ProcessDrawCardRequestSystem>());
            Add(systemFactory.Create<ProcessDrawCardFromStackRequestSystem>());
            Add(systemFactory.Create<CalculateHandLayoutOnRequestSystem>());
            Add(systemFactory.Create<ProcessAnimateCardPositionRequestSystem>());
            Add(systemFactory.Create<UpdateSelectedCardVisualSystem>());
            Add(systemFactory.Create<ProcessPlaceCardRequestSystem>());
        }
    }
}

