using Entitas;

namespace Code.Features.Statuses.Components
{
    [Game]
    public class StatusTypeIdComponent : IComponent
    {
        public StatusTypeId Value;
    }

    [Game]
    public class StatusComponent : IComponent
    {
        
    }

    [Game]
    public class StatusOwner : IComponent
    {
        public int Value;
    }

    [Game]
    public class StatusTarget : IComponent
    {
        public int Value;
    }

    [Game]
    public class DamageStatus : IComponent
    {
        
    }
}