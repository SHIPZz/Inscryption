using Code.Common.Collisions;
using Code.Features.View.Registrars;
using Code.Features.View.SelfInitialized;
using UnityEngine;
using Zenject;

namespace Code.Features.View
{
	public class EntityBehaviour : MonoBehaviour, IEntityView
	{
		private GameEntity _entity;
		private ICollisionRegistry _collisionRegistry;
		public GameEntity Entity => _entity;

		private bool _isRegistered;

		public bool IsSelfInitialized { get; private set; }

		[Inject]
		private void Construct(ICollisionRegistry collisionRegistry)
		{
			_collisionRegistry = collisionRegistry;
		}

		public void SetEntity(GameEntity entity)
		{
			if (_isRegistered)
				return;

			_entity = entity;
			_entity.AddView(this);
			_entity.Retain(this);

			foreach (IEntityComponentRegistrar registrar in GetComponentsInChildren<IEntityComponentRegistrar>())
			{
				if (registrar.EntityBehaviour == this)
					registrar.RegisterComponents();
			}

			foreach (Collider collider2d in GetComponentsInChildren<Collider>(includeInactive: true))
			{
				_collisionRegistry.Register(collider2d.GetInstanceID(), _entity);
			}

			if (_entity.isSelfInitializedView == false)
			{
				foreach (AbstractSelfInitializedEntityView selfInitializedView in GetComponentsInChildren<AbstractSelfInitializedEntityView>())
				{
					selfInitializedView.EntityBehaviour.IsSelfInitialized = true;
					selfInitializedView.SendSelfInitializeRequest();
				}
			}

			_isRegistered = true;
		}

		public void ReleaseEntity()
		{
			if (!_isRegistered)
				return;

			foreach (IEntityComponentRegistrar registrar in GetComponentsInChildren<IEntityComponentRegistrar>())
			{
				if (registrar.EntityBehaviour == this)
					registrar.UnregisterComponents();
			}

			foreach (Collider collider2d in GetComponentsInChildren<Collider>(includeInactive: true))
			{
				_collisionRegistry.Unregister(collider2d.GetInstanceID());
			}

			_entity.Release(this);
			_entity = null;

			_isRegistered = false;
			IsSelfInitialized = false;
		}
	}
}