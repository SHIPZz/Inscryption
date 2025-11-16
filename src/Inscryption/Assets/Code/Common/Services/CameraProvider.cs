using UnityEngine;

namespace Code.Common.Services
{
	public class CameraProvider : ICameraProvider
	{
		private Camera _mainCamera;

		public Camera MainCamera
		{
			get
			{
				if (_mainCamera == null)
				{
					_mainCamera = Camera.main;
				}

				return _mainCamera;
			}
		}
	}
}

