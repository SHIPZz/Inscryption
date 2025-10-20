using UnityEngine;

namespace Code.Features.View.Registrars
{
	public class TransformRegistrar : EntityComponentRegistrar
	{
		[SerializeField] private Transform _targetTransform;

		public Transform TargetTransform => _targetTransform;

		public override void RegisterComponents()
		{
			if (_targetTransform != null && Entity != null)
				Entity.AddTransformReference(_targetTransform);
		}

		public override void UnregisterComponents()
		{
			if (Entity != null && Entity.hasTransformReference)
				Entity.RemoveTransformReference();
		}

		public void SetPosition(Vector3 position)
		{
			if (_targetTransform != null)
				_targetTransform.position = position;
		}

		public void SetLocalPosition(Vector3 localPosition)
		{
			if (_targetTransform != null)
				_targetTransform.localPosition = localPosition;
		}

		public void SetRotation(Quaternion rotation)
		{
			if (_targetTransform != null)
				_targetTransform.rotation = rotation;
		}

		public void SetLocalRotation(Quaternion localRotation)
		{
			if (_targetTransform != null)
				_targetTransform.localRotation = localRotation;
		}

		public void SetScale(Vector3 scale)
		{
			if (_targetTransform != null)
				_targetTransform.localScale = scale;
		}

		public void SetParent(Transform parent, bool worldPositionStays = true)
		{
			if (_targetTransform != null)
				_targetTransform.SetParent(parent, worldPositionStays);
		}
	}
}

