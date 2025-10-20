using Entitas;

namespace Code.Common.Inputs
{
    [Input] public class InputComponent : IComponent{}
    
    [Input] public class CardClickRequest : IComponent { public int Value; }
    
    [Input] public class SlotClickRequest : IComponent { public int Value; }
}