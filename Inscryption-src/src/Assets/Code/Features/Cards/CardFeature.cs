using Code.Features.Cards.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Cards
{
    public class CardFeature : Feature
    {
        public CardFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<ProcessDrawCardRequestSystem>());
        }
    }
}

