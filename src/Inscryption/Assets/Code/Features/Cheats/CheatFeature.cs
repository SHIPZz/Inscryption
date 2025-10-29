using Code.Features.Cheats.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Cheats
{
    public sealed class CheatFeature : Feature
    {
        public CheatFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<CheatSystem>());
        }
    }
}