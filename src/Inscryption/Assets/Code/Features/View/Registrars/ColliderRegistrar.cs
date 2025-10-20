using UnityEngine;

namespace Code.Features.View.Registrars
{
	public class ColliderRegistrar : EntityComponentRegistrar
	{
		[SerializeField] private Collider _targetCollider;

		public Collider TargetCollider => _targetCollider;

		public override void RegisterComponents()
		{
			if (_targetCollider != null && Entity != null)
				Entity.AddColliderReference(_targetCollider);
		}

		public override void UnregisterComponents()
		{
			if (Entity != null && Entity.hasColliderReference)
				Entity.RemoveColliderReference();
		}

		private void OnValidate()
		{
			if (_targetCollider == null)
				_targetCollider = GetComponent<Collider>();
		}
	}
}

