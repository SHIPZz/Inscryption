using Code.Common.Extensions;
using Code.Common.Random;
using Code.Common.Services;
using Code.Features.Cards.Data;
using Code.Features.View;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Features.Cards.Services
{
    public class CardFactory : ICardFactory
    {
        private readonly GameContext _game;
        private readonly IIdService _idService;
        private readonly IRandomService _randomService;
        private readonly IInstantiateService _instantiateService;
        private readonly IAssetsService _assetsService;
        private readonly IConfigService _configService;

        public CardFactory(
            GameContext game, 
            IIdService idService, 
            IRandomService randomService,
            IInstantiateService instantiateService,
            IAssetsService assetsService,
            IConfigService configService)
        {
            _game = game;
            _idService = idService;
            _randomService = randomService;
            _instantiateService = instantiateService;
            _assetsService = assetsService;
            _configService = configService;
        }

        public GameEntity CreateCard(CardCreateData cardCreateData)
        {
            GameEntity card = _game.CreateEntity()
                .AddId(_idService.Next())
                .With(x => x.isCard = true)
                .With(x => x.AddHp(cardCreateData.Hp))
                .With(x => x.AddMaxHp(cardCreateData.Hp))
                .With(x => x.AddDamage(cardCreateData.Damage))
                .With(x => x.AddCardOwner(cardCreateData.OwnerId))
                .With(x => x.isInHand = cardCreateData.InHand)
                .With(x => x.AddCardIcon(cardCreateData.Icon), when: cardCreateData.Icon != null)
                .With(x => x.AddViewAddressableKey(cardCreateData.ViewKey), when: !string.IsNullOrEmpty(cardCreateData.ViewKey))
                // .With(x => x.isTrackCameraRotation = true)
                ;

            if (cardCreateData.Icon != null)
            {
                CreateView(card, cardCreateData);
            }

            return card;
        }

        public GameEntity CreateRandomCard(CardCreateData cardCreateData)
        {
            CardConfig cardConfig = _configService.GetConfig<CardConfig>(nameof(CardConfig));
            
            if (cardConfig == null || cardConfig.Cards == null || cardConfig.Cards.Count == 0)
            {
                Debug.LogError("[CardFactory] CardConfig or Cards list is empty!");
                return null;
            }

            CardData randomCardData = cardConfig.Cards[_randomService.Range(0, cardConfig.Cards.Count - 1)];
            
            return CreateCard(new CardCreateData(
                cardCreateData.OwnerId, 
                randomCardData.Hp, 
                randomCardData.Damage, 
                inHand: cardCreateData.InHand, 
                icon: cardCreateData.Icon ?? randomCardData.VisualData?.Icon,
                viewKey: null,
                position: cardCreateData.Position,
                rotation: cardCreateData.Rotation));
        }

        private void CreateView(GameEntity card, CardCreateData cardCreateData)
        {
            CardEntityView cardPrefab = _assetsService.LoadPrefabWithComponent<CardEntityView>(nameof(CardEntityView));
            
            if (cardPrefab != null)
            {
                CardEntityView cardView = _instantiateService.Instantiate(cardPrefab, cardCreateData.Position, cardCreateData.Rotation);
                
                if (cardView != null && cardView.EntityBehaviour != null)
                {
                    cardView.EntityBehaviour.SetEntity(card);
                    cardView.SetIcon(cardCreateData.Icon);
                    card.ReplaceView(cardView.EntityBehaviour);
                    
                    if (cardView.CardAnimator != null)
                    {
                        card.AddCardAnimator(cardView.CardAnimator);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"[CardFactory] Card prefab not found in Addressables: {nameof(CardEntityView)}");
            }
        }
    }

    public struct CardCreateData
    {
        public readonly int OwnerId;
        public readonly int Hp;
        public readonly int Damage;
        public readonly bool InHand;
        public readonly Sprite Icon;
        public readonly string ViewKey;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;

        public CardCreateData(int ownerId, int hp, int damage, bool inHand = false, Sprite icon = null, string viewKey = null, Vector3 position = default, Quaternion rotation = default)
        {
            OwnerId = ownerId;
            Hp = hp;
            Damage = damage;
            InHand = inHand;
            Icon = icon;
            ViewKey = viewKey;
            Position = position;
            Rotation = rotation == default ? Quaternion.identity : rotation;
        }
    }
}


