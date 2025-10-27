using Code.Features.Board.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Board
{
    public class BoardFeature : Feature
    {
        public BoardFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<ProcessPlaceCardRequestSystem>());
        }
    }
}