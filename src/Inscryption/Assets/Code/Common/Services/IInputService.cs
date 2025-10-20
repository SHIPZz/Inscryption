using UnityEngine;

namespace Code.Common.Services
{
	public interface IInputService
	{
		bool GetMouseButtonDown(int button);
		Vector3 MousePosition { get; }
	}
}

