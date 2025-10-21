using UnityEngine;

namespace Code.Features.View.Registrars
{
	public class TransformRegistrar : EntityComponentRegistrar
	{
		[SerializeField] private Transform _targetTransform;

		public override void RegisterComponents()
		{
			if (_targetTransform != null && Entity != null)
				Entity.AddTransform(_targetTransform);
		}

		public override void UnregisterComponents()
		{
			if (Entity != null && Entity.hasTransform)
				Entity.RemoveTransform();
		}
	}
}

