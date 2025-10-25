using System.Collections.Generic;
using Entitas;
using UnityEngine;

[Game]
public class DrawCardFromStackRequest : IComponent
{
    public int StackEntityId;
    public int CardsToDraw;
    public int OwnerId;
    public IReadOnlyList<Vector3> TargetPositions;
    public float DelayBetweenCards;
    public float MoveDuration;
    public Transform Parent;
}
