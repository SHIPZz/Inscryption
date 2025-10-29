using UnityEngine;

namespace Code.Common.Services
{
	public interface IInputService
	{
		bool GetMouseButtonDown(int button);
		bool GetKeyDown(KeyCode key);
		Vector3 MousePosition { get; }
	}
}

