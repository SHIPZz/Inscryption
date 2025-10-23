using System.Collections.Generic;
using Code.Common.Random;
using Code.Common.Services;
using Code.Features.Layout.Services;
using Code.Infrastructure.Level;
using UnityEngine;

namespace Code.Features.Cards.Services
{
    public class CardStackFactory : ICardStackFactory
    {
        private readonly GameContext _game;
        private readonly IIdService _idService;
        private readonly ICardFactory _cardFactory;
        private readonly IRandomService _randomService;
        private readonly ILevelProvider _levelProvider;

        public CardStackFactory(GameContext game, IIdService idService, ICardFactory cardFactory, IRandomService randomService, ILevelProvider levelProvider)
        {
            _game = game;
            _idService = idService;
            _cardFactory = cardFactory;
            _randomService = randomService;
            _levelProvider = levelProvider;
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
            var cards = new List<GameEntity>();
            
            var layoutParams = new VerticalLayoutParams
            {
                Count = createData.CardCount,
                Spacing = createData.VerticalOffset,
                Origin = Vector3.zero
            };
            var cardLocalPositions = PositionCalculator.CalculateVerticalLayoutPositions(layoutParams);

            for (int i = 0; i < createData.CardCount; i++)
            {
                float randomYRotation = _randomService.Range(-25f, 25f);
                Quaternion cardRotation = Quaternion.Euler(90f, randomYRotation, 0f);

                GameEntity card = _cardFactory.CreateRandomCard(new CardCreateData(
                    ownerId: createData.OwnerId,
                    hp: 0,
                    damage: 0,
                    inHand: false,
                    icon: null,
                    viewKey: null,
                    position: createData.Position,
                    rotation: cardRotation,
                    isHeroOwner: createData.IsHero,
                    parent: _levelProvider.DeckStackParent
                ));
                
                card.AddLocalPosition(cardLocalPositions[i]);

                cards.Add(card);
            }

            return cards;
        }

        private void PopulateStack(GameEntity stack, List<GameEntity> cards)
        {
            foreach (GameEntity card in cards)
            {
                stack.cardStack.Value.Push(card.Id);
            }
        }
    }

    public struct CardStackCreateData
    {
        public readonly int CardCount;
        public readonly int OwnerId;
        public readonly Vector3 Position;
        public readonly float VerticalOffset;
        public readonly bool IsHero;

        public CardStackCreateData(int cardCount, int ownerId, Vector3 position, float verticalOffset, bool isHero)
        {
            CardCount = cardCount;
            OwnerId = ownerId;
            Position = position;
            VerticalOffset = verticalOffset;
            IsHero = isHero;
        }
    }
}
