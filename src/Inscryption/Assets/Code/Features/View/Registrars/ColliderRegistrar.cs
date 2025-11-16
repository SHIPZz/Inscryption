using UnityEngine;

namespace Code.Features.View.Registrars
{
	public class ColliderRegistrar : EntityComponentRegistrar
	{
		[SerializeField] private Collider _targetCollider;

		public override void RegisterComponents()
		{
			if (_targetCollider != null && Entity != null)
				Entity.AddCollider(_targetCollider);
		}

		public override void UnregisterComponents()
		{
			if (Entity != null && Entity.hasCollider)
				Entity.RemoveCollider();
		}

		private void OnValidate()
		{
			if (_targetCollider == null)
				_targetCollider = GetComponent<Collider>();
		}
	}
}

