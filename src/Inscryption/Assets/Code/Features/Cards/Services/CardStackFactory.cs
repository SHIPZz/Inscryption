using System.Collections.Generic;
using Code.Common.Services;
using UnityEngine;

namespace Code.Features.Cards.Services
{
    public class CardStackFactory : ICardStackFactory
    {
        private readonly GameContext _game;
        private readonly IIdService _idService;
        private readonly ICardFactory _cardFactory;

        public CardStackFactory(GameContext game, IIdService idService, ICardFactory cardFactory)
        {
            _game = game;
            _idService = idService;
            _cardFactory = cardFactory;
        }

        public GameEntity CreateCardStack(CardStackCreateData createData)
        {
            GameEntity stack = CreateStackEntity();
            List<GameEntity> cards = CreateCardsForStack(createData);
            PopulateStack(stack, cards);

            return stack;
        }

        private GameEntity CreateStackEntity()
        {
            GameEntity stack = _game.CreateEntity();
            stack.AddId(_idService.Next());
            stack.AddCardStack(new Stack<int>());
            return stack;
        }

        private List<GameEntity> CreateCardsForStack(CardStackCreateData createData)
        {
            List<GameEntity> cards = new List<GameEntity>();
            Quaternion cardRotation = Quaternion.Euler(0, 90, 0);

            for (int i = 0; i < createData.CardCount; i++)
            {
                Vector3 cardPosition = CalculateCardPosition(createData.Position, i, createData.VerticalOffset);
                GameEntity card = _cardFactory.CreateRandomCard(new CardCreateData(
                    ownerId: createData.OwnerId,
                    hp: 0,
                    damage: 0,
                    inHand: false,
                    icon: null,
                    viewKey: null,
                    position: cardPosition,
                    rotation: cardRotation
                ));

                cards.Add(card);
            }

            return cards;
        }

        private Vector3 CalculateCardPosition(Vector3 basePosition, int index, float verticalOffset)
        {
            return new Vector3(
                basePosition.x,
                basePosition.y + index * verticalOffset,
                basePosition.z
            );
        }

        private void PopulateStack(GameEntity stack, List<GameEntity> cards)
        {
            foreach (GameEntity card in cards)
            {
                stack.cardStack.Value.Push(card.Id);
            }
        }
    }
}
