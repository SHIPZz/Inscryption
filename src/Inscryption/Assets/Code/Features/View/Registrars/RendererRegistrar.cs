using UnityEngine;

namespace Code.Features.View.Registrars
{
	public class RendererRegistrar : EntityComponentRegistrar
	{
		[SerializeField] private Renderer _targetRenderer;

		public Renderer TargetRenderer => _targetRenderer;

		public override void RegisterComponents()
		{
			if (_targetRenderer != null && Entity != null)
				Entity.AddRenderer(_targetRenderer);
		}

		public override void UnregisterComponents()
		{
			if (Entity != null && Entity.hasRenderer)
				Entity.RemoveRenderer();
		}

		public void SetColor(Color color)
		{
			if (_targetRenderer != null && _targetRenderer.material != null)
			{
				_targetRenderer.material.color = color;
			}
		}

		private void OnValidate()
		{
			if (_targetRenderer == null)
				_targetRenderer = GetComponent<Renderer>();
		}
	}
}

