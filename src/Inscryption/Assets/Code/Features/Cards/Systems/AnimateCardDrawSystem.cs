using System.Collections.Generic;
using Code.Infrastructure.Level;
using DG.Tweening;
using Entitas;
using UnityEngine;

namespace Code.Features.Cards.Systems
{
    public class AnimateCardDrawSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly ILevelProvider _levelProvider;
        private readonly IGroup<GameEntity> _cards;
        private readonly List<GameEntity> _buffer = new(32);

        private const float MoveToParentDuration = 0.3f;
        private const float RotationDuration = 0.2f;

        public AnimateCardDrawSystem(GameContext game, ILevelProvider levelProvider)
        {
            _game = game;
            _levelProvider = levelProvider;

            _cards = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Card, GameMatcher.InHand, GameMatcher.Transform)
                .NoneOf(GameMatcher.AnimatedToHand));
        }

        public void Execute()
        {
            foreach (GameEntity card in _cards.GetEntities(_buffer))
            {
                if (card.hasCardOwner && card.hasTransform)
                {
                    AnimateCardToHand(card);
                    card.isAnimatedToHand = true;
                }
            }
        }

        private void AnimateCardToHand(GameEntity card)
        {
            Transform cardTransform = card.Transform;
            if (cardTransform == null)
                return;

            GameEntity owner = _game.GetEntityWithId(card.CardOwner);
            if (owner == null)
                return;

            Transform targetParent = owner.isHero
                ? _levelProvider.HeroCardParent
                : _levelProvider.EnemyCardParent;

            if (targetParent == null)
            {
                Debug.LogWarning($"[AnimateCardDrawSystem] Target parent not found for card {card.Id}");
                return;
            }

            Vector3 targetPosition = cardTransform.position;
            targetPosition.y = targetParent.position.y;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(cardTransform.DOMove(targetPosition, MoveToParentDuration).SetEase(Ease.OutQuad));
            sequence.Append(cardTransform.DORotateQuaternion(Quaternion.identity, RotationDuration).SetEase(Ease.OutQuad));

            sequence.Play();

            Debug.Log($"[AnimateCardDrawSystem] Animating card {card.Id} to hand");
        }
    }
}
