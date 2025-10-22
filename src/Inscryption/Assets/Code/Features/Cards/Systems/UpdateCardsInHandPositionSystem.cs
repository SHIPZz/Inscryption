using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Features.Cards.Systems
{
    public class UpdateCardsInHandPositionSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _owners;
        private readonly List<GameEntity> _buffer = new(8);

        public UpdateCardsInHandPositionSystem(GameContext game)
        {
            _game = game;
            _owners = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.CardsInHand, GameMatcher.HandPosition, GameMatcher.CardHorizontalOffset)
                .NoneOf(GameMatcher.Destructed));
        }

        public void Execute()
        {
            foreach (GameEntity owner in _owners.GetEntities(_buffer))
            {
                UpdateHandPositions(owner);
            }
        }

        private void UpdateHandPositions(GameEntity owner)
        {
            List<int> cardsInHand = owner.cardsInHand.Value;

            if (IsHandEmpty(cardsInHand))
                return;

            Vector3 handPosition = owner.handPosition.Value;
            float horizontalOffset = owner.cardHorizontalOffset.Value;
            float startX = CalculateStartPosition(handPosition.x, cardsInHand.Count, horizontalOffset);

            PositionCards(cardsInHand, handPosition, startX, horizontalOffset);
        }

        private bool IsHandEmpty(List<int> cardsInHand)
        {
            return cardsInHand.Count == 0;
        }

        private float CalculateStartPosition(float centerX, int cardCount, float offset)
        {
            float totalWidth = (cardCount - 1) * offset;
            return centerX - totalWidth / 2f;
        }

        private void PositionCards(List<int> cardsInHand, Vector3 handPosition, float startX, float horizontalOffset)
        {
            for (int i = 0; i < cardsInHand.Count; i++)
            {
                GameEntity card = _game.GetEntityWithId(cardsInHand[i]);

                if (IsValidCard(card))
                {
                    Vector3 targetPosition = CalculateCardPosition(startX, i, horizontalOffset, handPosition);
                    card.ReplaceWorldPosition(targetPosition);
                }
            }
        }

        private bool IsValidCard(GameEntity card)
        {
            return card != null && card.hasWorldPosition;
        }

        private Vector3 CalculateCardPosition(float startX, int index, float offset, Vector3 handPosition)
        {
            return new Vector3(
                startX + index * offset,
                handPosition.y,
                handPosition.z
            );
        }
    }
}
