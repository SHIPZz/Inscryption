using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Features.Cards
{
    [Game]
    public class Card : IComponent
    {
    }

    [Game]
    public class CardStack : IComponent
    {
        public Stack<int> Value;
    }

    [Game]
    public class CardOwner : IComponent
    {
        public int Value;
    }
    
    [Game]
    public class HeroOwner : IComponent
    {
    }
    
    [Game]
    public class PlacedCards : IComponent
    {
        public List<int> Value;
    }
    
    [Game]
    public class EnemyOwner : IComponent
    {
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
    public class AllCardDelivered : IComponent
    {
    }

    [Game]
    public class Lane : IComponent
    {
        public int Value;
    }

    [Game]
    public class CardIcon : IComponent
    {
        public Sprite Value;
    }
    
    [Game] public class Selected : IComponent {}
    
    [Game] public class SelectionAvailable : IComponent {}
    
    [Game] public class SlotId : IComponent { public int Value; }
    
    [Game] public class Placed : IComponent { }

    [Game]
    public class CardAnimatorComponent : IComponent
    {
        public CardAnimator Value;
    }

    [Game]
    public class VisualTransform : IComponent
    {
        public Transform Value;
    }
}

