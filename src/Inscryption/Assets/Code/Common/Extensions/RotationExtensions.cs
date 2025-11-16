using UnityEngine;

namespace Code.Common.Extensions
{
	public static class RotationExtensions
	{
		public static Quaternion GetLookRotationTo(this Vector3 from, Vector3 to, bool ignoreY = true)
		{
			Vector3 direction = to - from;

			if (ignoreY)
				direction.y = 0;

			if (direction.sqrMagnitude > 0.001f)
			{
				return Quaternion.LookRotation(direction);
			}

			return Quaternion.identity;
		}
	}
}

