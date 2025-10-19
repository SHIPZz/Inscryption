using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Common
{
    [Game] public class ViewPath : IComponent { public string Value; }
    
    [Game, Meta] public class Destructed : IComponent { }
    
    [Game] public class Active : IComponent { }
    
    [Game] public class LayerMaskComponent : IComponent { public int Value; }
    [Game,Unique] public class Id : IComponent { [PrimaryEntityIndex] public int Value; }
    
    [Game] public class SelfDestructTimer : IComponent { public float Value; }
    
    [Game] public class Hp : IComponent { public int Value; }
    
    [Game] public class MaxHp : IComponent { public int Value; }
}