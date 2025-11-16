using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Common.Random;
using Code.Common.Services;
using Code.Features.Cards.Data;
using Code.Features.Stats;
using Code.Infrastructure.Data;
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
        private readonly GameConfig _gameConfig;
        private readonly CardConfig _cardConfig;

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
            _gameConfig = configService.GetConfig<GameConfig>();
            _cardConfig = configService.GetConfig<CardConfig>();
        }

        public GameEntity CreateCard(CardCreateData data)
        {
            GameEntity card = CreateCardEntity(data);

            if (data.Icon != null)
                CreateView(card, data);

            return card;
        }

        private GameEntity CreateCardEntity(CardCreateData data)
        {
            int hp = Mathf.Clamp(data.Hp, _gameConfig.CardGeneration.HpRange.x, _gameConfig.CardGeneration.HpRange.y);
            int damage = Mathf.Clamp(data.Damage, _gameConfig.CardGeneration.DamageRange.x, _gameConfig.CardGeneration.DamageRange.y);

            GameEntity card = _game.CreateEntity()
                .AddId(_idService.Next())
                .With(x => x.isCard = true);

            AddCardStats(card, hp, damage);
            AddCardOwnership(card, data);
            AddCardVisuals(card, data);

            return card;
        }

        private void AddCardStats(GameEntity card, int hp, int damage)
        {
            card.AddHp(hp);
            card.AddMaxHp(hp);
            card.AddStats(new Dictionary<StatTypeId, int> { { StatTypeId.Hp, hp } });
            card.AddStatsModifiers(new Dictionary<StatTypeId, int>());
            card.AddDamage(damage);
        }

        private void AddCardOwnership(GameEntity card, CardCreateData data)
        {
            card.AddCardOwner(data.OwnerId);
            card.isInHand = data.InHand;
        }

        private void AddCardVisuals(GameEntity card, CardCreateData data)
        {
            card.AddName("Card");

            if (data.Icon != null)
                card.AddCardIcon(data.Icon);

            if (!string.IsNullOrEmpty(data.ViewKey))
                card.AddViewAddressableKey(data.ViewKey);
        }

        public GameEntity CreateRandomCard(CardCreateData data)
        {
            CardData randomData = GetRandomCardData();
            return randomData == null ? null : CreateCard(MergeWithRandomData(data, randomData));
        }

        private CardData GetRandomCardData()
        {
            if (_cardConfig?.Cards == null || _cardConfig.Cards.Count == 0)
            {
                Debug.LogError("[CardFactory] CardConfig or Cards list is empty!");
                return null;
            }

            return _cardConfig.Cards[_randomService.Range(0, _cardConfig.Cards.Count - 1)];
        }

        private CardCreateData MergeWithRandomData(CardCreateData original, CardData random)
        {
            return new CardCreateData(
                    original.OwnerId,
                    random.Hp,
                    random.Damage,
                    inHand: original.InHand,
                    icon: original.Icon ?? random.VisualData?.Icon,
                    viewKey: null,
                    position: original.Position,
                    rotation: original.Rotation,
                    parent: original.Parent);
        }

        private void CreateView(GameEntity card, CardCreateData data)
        {
            CardEntityView view = InstantiateCardView(data);
            if (view == null)
                return;

            LinkViewToEntity(card, view, data.Icon);
            SetupViewComponents(card, view);

            if (data.Parent != null)
                card.ReplaceParent(data.Parent);
        }

        private CardEntityView InstantiateCardView(CardCreateData data)
        {
            CardEntityView prefab = _assetsService.LoadPrefabWithComponent<CardEntityView>(nameof(CardEntityView));
            
            if (prefab == null)
            {
                Debug.LogWarning($"[CardFactory] Card prefab not found: {nameof(CardEntityView)}");
                return null;
            }

            return _instantiateService.Instantiate(prefab, data.Position, data.Rotation);
        }

        private void LinkViewToEntity(GameEntity card, CardEntityView view, Sprite icon)
        {
            if (view.EntityBehaviour == null)
                return;

            view.EntityBehaviour.SetEntity(card);
            view.SetIcon(icon);
            card.ReplaceView(view.EntityBehaviour);
        }

        private void SetupViewComponents(GameEntity card, CardEntityView view)
        {
            if (view.CardAnimator == null)
                return;

            card.AddCardAnimator(view.CardAnimator);
            card.AddDamageAnimator(view.CardAnimator);
            card.AddAttackAnimator(view.CardAnimator);
            card.AddVisualTransform(view.CardAnimator.VisualTransform);
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
        public readonly Transform Parent;

        public CardCreateData(int ownerId, int hp, int damage, bool inHand = false, Sprite icon = null,
            string viewKey = null, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            OwnerId = ownerId;
            Hp = hp;
            Damage = damage;
            InHand = inHand;
            Icon = icon;
            ViewKey = viewKey;
            Position = position;
            Rotation = rotation == default ? Quaternion.identity : rotation;
            Parent = parent;
        }
    }
}