using Entitas;
using UnityEngine;

namespace Code.Features.Cards.Components
{
    [Game]
    public class AnimateCardPositionRequest : IComponent
    {
        public int CardId;
        public Vector3 Position;
        public Quaternion Rotation;
    }
}
