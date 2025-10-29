using Entitas;
using UnityEngine;

namespace Code.Features.Cards.Components
{
    [Game]
    public class DrawCardFromStackRequest : IComponent
    {
        public int StackEntityId;
        public int CardsToDraw;
        public int OwnerId;
        public Vector3 TargetPosition;
        public float DelayBetweenCards;
        public float MoveDuration;
        public Transform Parent;
    }
}
