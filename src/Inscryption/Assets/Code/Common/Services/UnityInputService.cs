using UnityEngine;

namespace Code.Common.Services
{
	public class UnityInputService : IInputService
	{
		public bool GetMouseButtonDown(int button) => Input.GetMouseButtonDown(button);
		public bool GetKeyDown(KeyCode key) => Input.GetKeyDown(key);

		public Vector3 MousePosition => Input.mousePosition;
	}
}

