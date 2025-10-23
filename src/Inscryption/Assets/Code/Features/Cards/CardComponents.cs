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
    public class Lane : IComponent
    {
        public int Value;
    }

    [Game]
    public class CardIcon : IComponent
    {
        public Sprite Value;
    }

    [Game]
    public class CardAnimatorComponent : IComponent
    {
        public CardAnimator Value;
    }

    [Game]
    public class DrawCardFromStackRequest : IComponent
    {
        public int StackId;
        public int OwnerId;
    }

    [Game]
    public class AnimatedToHand : IComponent
    {
    }
}

