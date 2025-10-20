using Code.Common;
using Code.Common.Extensions;
using Code.Common.Services;
using UnityEngine;
using Zenject;

namespace Code.Features.View.SelfInitialized
{
	public abstract class AbstractSelfInitializedEntityView : MonoBehaviour
	{
		public EntityBehaviour EntityBehaviour;
		
		private IIdService _identifiers;

		[Inject]
		private void Construct(IIdService identifiers) =>
			_identifiers = identifiers;

		public void SendSelfInitializeRequest()
		{
			CreateEntity.Empty().AddSelfInitializeEntityViewRequest(this);
		}

		public void Initialize()
		{
			GameEntity entity = BuildEntity();
			entity
				.With(x => x.AddId(_identifiers.Next()), when: !entity.hasId)
				.With(x => x.isSelfInitializedView = true);

			EntityBehaviour.SetEntity(entity);
		}

		protected abstract GameEntity BuildEntity();
	}
}