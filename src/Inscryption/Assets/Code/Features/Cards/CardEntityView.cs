using Code.Common.Extensions;
using Code.Features.View;
using Code.Features.View.Registrars;
using UnityEngine;

namespace Code.Features.Cards
{
	[RequireComponent(typeof(ColliderRegistrar))]
	[RequireComponent(typeof(CardAnimator))]
	public class CardEntityView : MonoBehaviour
	{
		[SerializeField] private EntityBehaviour _entityBehaviour;
		[SerializeField] private SpriteRenderer _iconRenderer;
		[SerializeField] private ColliderRegistrar _colliderRegistrar;
		[SerializeField] private CardAnimator _cardAnimator;

		public EntityBehaviour EntityBehaviour => _entityBehaviour;
		public GameEntity Entity => _entityBehaviour.Entity;
		public CardAnimator CardAnimator => _cardAnimator;

		public void SetIcon(Sprite icon)
		{
			if (_iconRenderer != null)
				_iconRenderer.sprite = icon;
		}

		private void OnValidate()
		{
			if (_entityBehaviour == null)
				_entityBehaviour = this.GetComponentAnyWhere<EntityBehaviour>();

			if (_iconRenderer == null)
				_iconRenderer = this.GetComponentAnyWhere<SpriteRenderer>(true);

			if (_colliderRegistrar == null)
				_colliderRegistrar = GetComponent<ColliderRegistrar>();

			if (_cardAnimator == null)
				_cardAnimator = GetComponent<CardAnimator>();
		}
	}
}

