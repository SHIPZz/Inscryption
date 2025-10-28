using System.Collections.Generic;
using Code.Common.Random;
using Code.Common.Services;
using Code.Features.Layout.Services;
using Code.Infrastructure.Data;
using Code.Infrastructure.Level;
using Code.Infrastructure.Services;
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
        private readonly IConfigService _configService;
        private GameConfig _gameConfig;

        public CardStackFactory(GameContext game, IIdService idService, ICardFactory cardFactory,
            IRandomService randomService, ILevelProvider levelProvider, IConfigService configService)
        {
            _game = game;
            _idService = idService;
            _cardFactory = cardFactory;
            _randomService = randomService;
            _levelProvider = levelProvider;
            _configService = configService;
            _gameConfig = _configService.GetConfig<GameConfig>();
        }

        public GameEntity CreateCardStack(CardStackCreateData createData)
        {
            GameEntity stack = CreateStackEntity(createData);

            return stack;
        }

        private GameEntity CreateStackEntity(CardStackCreateData data)
        {
            GameEntity stack = _game.CreateEntity();
            stack.AddId(_idService.Next());
            stack.AddCardStack(new Stack<int>(32));
            CreateCardsForStack(data, stack);
            return stack;
        }

        private void CreateCardsForStack(CardStackCreateData createData, GameEntity stack)
        {
            var layoutParams = new VerticalLayoutParams
            {
                Count = createData.CardCount,
                Spacing = createData.VerticalOffset,
                Origin = Vector3.zero
            };

            IReadOnlyList<Vector3> cardLocalPositions = PositionCalculator.CalculateVerticalLayoutPositions(layoutParams);
            var rotationRange = _gameConfig.CardGeneration.RotationRange;

            for (int i = 0; i < createData.CardCount; i++)
            {
                float randomYRotation = _randomService.Range(rotationRange.x, rotationRange.y);
                Quaternion cardRotation = Quaternion.Euler(90f, randomYRotation, 0f);

                GameEntity card = _cardFactory.CreateRandomCard(new CardCreateData(
                    ownerId: stack.Id,
                    hp: 0,
                    damage: 0,
                    inHand: false,
                    icon: null,
                    viewKey: null,
                    position: createData.Position,
                    rotation: cardRotation,
                    parent: _levelProvider.DeckStackParent
                ));

                card.AddLocalPosition(cardLocalPositions[i]);

                stack.CardStack.Push(card.Id);
            }
        }

        public struct CardStackCreateData
        {
            public readonly int CardCount;
            public readonly Vector3 Position;
            public readonly float VerticalOffset;

            public CardStackCreateData(int cardCount, Vector3 position, float verticalOffset)
            {
                CardCount = cardCount;
                Position = position;
                VerticalOffset = verticalOffset;
            }
        }
    }
}

