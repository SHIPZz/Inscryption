using Entitas;

namespace Code.Features.Cooldowns
{
    [Game]
    public class CooldownLeft : IComponent { public float Value; }
    
    [Game]
    public class CooldownUp : IComponent { }
    
    [Game]
    public class Cooldown : IComponent { public float Value; }
}