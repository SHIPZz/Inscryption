using UnityEngine;

namespace Code.Features.View.Registrars
{
	public class SpriteRendererRegistrar : EntityComponentRegistrar
	{
		[SerializeField] private SpriteRenderer _spriteRenderer;

		public SpriteRenderer SpriteRenderer => _spriteRenderer;

		public override void RegisterComponents()
		{
			if (_spriteRenderer != null && Entity != null)
				Entity.AddSpriteRendererReference(_spriteRenderer);
		}

		public override void UnregisterComponents()
		{
			if (Entity != null && Entity.hasSpriteRendererReference)
				Entity.RemoveSpriteRendererReference();
		}
	}
}

