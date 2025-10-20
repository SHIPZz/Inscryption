using UnityEngine;

namespace Code.Common.Services
{
	public class UnityInputService : IInputService
	{
		public bool GetMouseButtonDown(int button)
		{
			return Input.GetMouseButtonDown(button);
		}

		public Vector3 MousePosition => Input.mousePosition;
	}
}

