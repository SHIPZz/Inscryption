using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Code.Common
{
    [Game, Meta] public class Destructed : IComponent { }
    
    [Game] public class Active : IComponent { }
    
    [Game,Input] public class LayerMaskComponent : IComponent { public int Value; }
    [Game,Unique] public class Id : IComponent { [PrimaryEntityIndex] public int Value; }
    
    [Game] public class SelfDestructTimer : IComponent { public float Value; }
    
    [Game] public class Hp : IComponent { public int Value; }
    
    [Game] public class MaxHp : IComponent { public int Value; }
    
    [Game] public class Damage : IComponent { public int Value; }
    
    [Game] public class TransformComponent : IComponent { public Transform Value; }
    
    [Game] public class SpriteRendererComponent : IComponent { public SpriteRenderer Value; }
    
    [Game] public class RendererComponent : IComponent { public Renderer Value; }
    
    [Game] public class ColliderComponent : IComponent { public Collider Value; }
    
    [Game] public class NameComponent : IComponent { public string Value; }
    
    [Game] public class TrackCameraRotation : IComponent { }
    
    [Game, Unique] public class SelectedCards : IComponent { public System.Collections.Generic.List<int> Value; }
    
    [Game] public class WorldPosition : IComponent { public UnityEngine.Vector3 Value; }
    
    [Game] public class WorldRotation : IComponent { public UnityEngine.Quaternion Value; }
    
    [Game] public class LocalRotation : IComponent { public UnityEngine.Quaternion Value; }
}