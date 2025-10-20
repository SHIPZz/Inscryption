using System.Collections.Generic;
using Entitas;

namespace Code.Features.Hero
{
    [Game]
    public class Hero : IComponent
    {
    }

    [Game]
    public class HeroTurn : IComponent
    {
    }

    [Game]
    public class CardsInHand : IComponent
    {
        public List<int> Value;
    }
}

