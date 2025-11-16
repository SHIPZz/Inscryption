using Code.Common.Extensions;
using Code.Features.View;
using Code.Features.View.Registrars;
using UnityEngine;

namespace Code.Features.Board
{
	[RequireComponent(typeof(ColliderRegistrar))]
	[RequireComponent(typeof(RendererRegistrar))]
	public class SlotEntityView : MonoBehaviour
	{
		[SerializeField] private EntityBehaviour _entityBehaviour;
		[SerializeField] private ColliderRegistrar _colliderRegistrar;
		[SerializeField] private RendererRegistrar _rendererRegistrar;

		public EntityBehaviour EntityBehaviour => _entityBehaviour;
		public GameEntity Entity => _entityBehaviour.Entity;

		public void SetColor(Color color)
		{
			if (_rendererRegistrar != null)
			{
				_rendererRegistrar.SetColor(color);
			}
		}

		private void OnValidate()
		{
			if (_entityBehaviour == null)
				_entityBehaviour = this.GetComponentAnyWhere<EntityBehaviour>();

			if (_colliderRegistrar == null)
				_colliderRegistrar = GetComponent<ColliderRegistrar>();

			if (_rendererRegistrar == null)
				_rendererRegistrar = GetComponent<RendererRegistrar>();
		}
	}
}

