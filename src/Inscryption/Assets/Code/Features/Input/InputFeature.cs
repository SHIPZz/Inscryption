using Code.Features.Input.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Input
{
	public class InputFeature : Feature
	{
		public InputFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<DetectClicksSystem>());
			Add(systemFactory.Create<ProcessCardClickRequestSystem>());
			Add(systemFactory.Create<ProcessSlotClickToPlaceRequestSystem>());
		}
	}
}

