using Code.Features.Cooldowns.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Cooldowns
{
    public class CooldownFeature : Feature
    {
        public CooldownFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<CalculateCooldownSystem>());
        }
    }
}