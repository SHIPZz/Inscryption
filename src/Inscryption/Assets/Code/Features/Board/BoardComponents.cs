using Entitas;

namespace Code.Features.Board
{
    [Game]
    public class BoardSlot : IComponent
    {
    }

    [Game]
    public class SlotLane : IComponent
    {
        public int Value;
    }

    [Game]
    public class SlotOwner : IComponent
    {
        public int Value;
    }

    [Game]
    public class OccupiedBy : IComponent
    {
        public int Value;
    }
    
    [Game]
    public class Occupied : IComponent
    {
    }
}

