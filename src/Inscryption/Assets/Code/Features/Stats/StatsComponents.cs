using System.Collections.Generic;
using Entitas;

namespace Code.Features.Stats
{
    [Game]
    public class Stats : IComponent
    {
        public Dictionary<StatTypeId, int> Value;
    }

    [Game]
    public class StatsModifiers : IComponent
    {
        public Dictionary<StatTypeId, int> Value;
    }
}


