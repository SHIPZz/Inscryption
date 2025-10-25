using Code.Common.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Movement
{
    public sealed class TimerFeature : Feature
    {
        public TimerFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<ProcessTimerSystem>());
        }
    }
}