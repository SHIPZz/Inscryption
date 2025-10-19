using Entitas;

namespace Code.Features.Cards
{
    [Game]
    public class Card : IComponent
    {
    }

    [Game]
    public class CardOwner : IComponent
    {
        public int Value;
    }

    [Game]
    public class InHand : IComponent
    {
    }

    [Game]
    public class OnBoard : IComponent
    {
    }

    [Game]
    public class Lane : IComponent
    {
        public int Value;
    }
}

