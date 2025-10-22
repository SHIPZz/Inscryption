using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Common.Random;
using Code.Features.Board.Services;
using Code.Features.Cards.Data;
using Code.Features.Cards.Services;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Game.Systems
{
    public class InitializeGameSystem : IInitializeSystem
    {
        private readonly GameContext _game;
        private readonly IHeroFactory _heroFactory;
        private readonly IEnemyFactory _enemyFactory;
        private readonly ICardFactory _cardFactory;
        private readonly ICardStackFactory _cardStackFactory;
        private readonly IBoardFactory _boardFactory;
        private readonly IConfigService _configService;
        private readonly IRandomService _randomService;

        private GameConfig _gameConfig;
        private BoardConfig _boardConfig;

        public InitializeGameSystem(
            GameContext game,
            IHeroFactory heroFactory,
            IEnemyFactory enemyFactory,
            ICardFactory cardFactory,
            ICardStackFactory cardStackFactory,
            IBoardFactory boardFactory,
            IConfigService configService,
            IRandomService randomService)
        {
            _game = game;
            _heroFactory = heroFactory;
            _enemyFactory = enemyFactory;
            _cardFactory = cardFactory;
            _cardStackFactory = cardStackFactory;
            _boardFactory = boardFactory;
            _configService = configService;
            _randomService = randomService;
        }

        public void Initialize()
        {
            Debug.Log("[InitializeGameSystem] Starting game initialization...");

            _gameConfig = _configService.GetConfig<GameConfig>(nameof(GameConfig));
            _boardConfig = _configService.GetConfig<BoardConfig>(nameof(BoardConfig));
            
            if (_gameConfig == null)
            {
                Debug.LogError("[InitializeGameSystem] GameConfig not found! Using default values.");
                return;
            }

            if (_boardConfig == null)
            {
                Debug.LogError("[InitializeGameSystem] BoardConfig not found!");
                return;
            }

            GameEntity hero = _heroFactory.CreateHero(_gameConfig.BaseHeroHealth);
            Debug.Log($"[InitializeGameSystem] Hero created: ID={hero.Id}, HP={hero.Hp}");

            GameEntity enemy = _enemyFactory.CreateEnemy(_gameConfig.BaseEnemyHealth);
            Debug.Log($"[InitializeGameSystem] Enemy created: ID={enemy.Id}, HP={enemy.Hp}");

            List<GameEntity> slots = _boardFactory.CreateSlots(hero.Id, enemy.Id);
            Debug.Log($"[InitializeGameSystem] Board created: {slots.Count} slots");

            List<DeckCard> deck = CreateDeck(hero.Id, enemy.Id);
            Debug.Log($"[InitializeGameSystem] Deck created: {deck.Count} cards");

            GameEntity heroStack = CreateCardStack(hero.Id, isHero: true);
            Debug.Log($"[InitializeGameSystem] Hero card stack created: ID={heroStack.Id}");

            GameEntity enemyStack = CreateCardStack(enemy.Id, isHero: false);
            Debug.Log($"[InitializeGameSystem] Enemy card stack created: ID={enemyStack.Id}");

            DealStartingHand(hero, deck, _gameConfig.StartingHandSize,true);
            DealStartingHand(enemy, deck, _gameConfig.StartingHandSize);

            Debug.Log("[InitializeGameSystem] Game initialization complete. Hero's turn!");
        }

        private List<DeckCard> CreateDeck(int heroId, int enemyId)
        {
            List<DeckCard> deck = new List<DeckCard>();
            CardConfig cardConfig = _configService.GetConfig<CardConfig>(nameof(CardConfig));

            if (cardConfig == null || cardConfig.Cards == null || cardConfig.Cards.Count == 0)
            {
                Debug.LogError("[InitializeGameSystem] CardConfig or Cards list is empty!");
                return deck;
            }

            for (int i = 0; i < _gameConfig.DeckSize / 2; i++)
            {
                CardData heroCardData = cardConfig.Cards[_randomService.Range(0, cardConfig.Cards.Count - 1)];
                CardData enemyCardData = cardConfig.Cards[_randomService.Range(0, cardConfig.Cards.Count - 1)];

                deck.Add(new DeckCard(heroId, heroCardData));
                deck.Add(new DeckCard(enemyId, enemyCardData));
            }

            return deck;
        }

        private void DealStartingHand(GameEntity player, List<DeckCard> deck, int count, bool heroOwner = false)
        {
            int cardsDealt = 0;

            for (int i = 0; i < count; i++)
            {
                if (deck.Count == 0)
                    break;

                int cardIndexInDeck = FindCardIndexInDeck(deck, player.Id);

                if (cardIndexInDeck == -1)
                    break;

                DeckCard deckCard = deck[cardIndexInDeck];
                deck.RemoveAt(cardIndexInDeck);

                GameEntity card = CreateCardInHand(player, deckCard, cardsDealt, heroOwner);
                player.CardsInHand.Add(card.Id);

                cardsDealt++;
            }

            Debug.Log($"[InitializeGameSystem] Player {player.Id} received {cardsDealt} cards");
        }

        private GameEntity CreateCardInHand(GameEntity player, DeckCard deckCard, int handIndex, bool heroOwner)
        {
            Vector3 handPosition = GetHandPosition(player.isHero, handIndex);

            return _cardFactory.CreateCard(new CardCreateData(
                    deckCard.OwnerId,
                    deckCard.CardData.Hp,
                    deckCard.CardData.Damage,
                    inHand: true,
                    icon: deckCard.CardData.VisualData?.Icon,
                    position: handPosition))
                .With(x => x.isHeroOwner = heroOwner)
                .With(x => x.isEnemyOwner = !heroOwner);
        }

        private Vector3 GetHandPosition(bool isHero, int cardIndex)
        {
            return isHero
                ? _boardConfig.GetHeroHandPosition(cardIndex)
                : _boardConfig.GetEnemyHandPosition(cardIndex);
        }

        private int FindCardIndexInDeck(List<DeckCard> deck, int ownerId)
        {
            for (int i = 0; i < deck.Count; i++)
            {
                if (deck[i].OwnerId == ownerId)
                    return i;
            }

            return -1;
        }

        private GameEntity CreateCardStack(int ownerId, bool isHero)
        {
            Vector3 stackPosition = isHero
                ? _boardConfig.GetHeroCardStackPosition()
                : _boardConfig.GetEnemyCardStackPosition();

            return _cardStackFactory.CreateCardStack(new CardStackCreateData(
                cardCount: 10,
                ownerId: ownerId,
                position: stackPosition,
                verticalOffset: 0.05f
            ));
        }

        private struct DeckCard
        {
            public readonly int OwnerId;
            public readonly CardData CardData;

            public DeckCard(int ownerId = -1, CardData cardData = null)
            {
                OwnerId = ownerId;
                CardData = cardData;
            }
        }
    }
}

