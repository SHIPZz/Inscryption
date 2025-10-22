using UnityEngine;

namespace Code.Features.Cards.Services
{
    public interface ICardStackFactory
    {
        GameEntity CreateCardStack(CardStackCreateData createData);
    }

    public struct CardStackCreateData
    {
        public readonly int CardCount;
        public readonly int OwnerId;
        public readonly Vector3 Position;
        public readonly float VerticalOffset;

        public CardStackCreateData(int cardCount, int ownerId, Vector3 position, float verticalOffset = 0.05f)
        {
            CardCount = cardCount;
            OwnerId = ownerId;
            Position = position;
            VerticalOffset = verticalOffset;
        }
    }
}