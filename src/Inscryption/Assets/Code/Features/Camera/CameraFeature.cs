using Code.Features.Camera.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Camera
{
	public class CameraFeature : Feature
	{
		public CameraFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<AlignRotationTowardsCameraSystem>());
		}
	}
}

