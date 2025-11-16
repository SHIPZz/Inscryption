using UnityEngine;

namespace Code.Common.Extensions
{
	public static class ComponentExtensions
	{
		public static T GetComponentAnyWhere<T>(this Component component, bool includeInactive = false) where T : Component
		{
			T result = component.GetComponent<T>();
			
			if (result != null)
				return result;
			
			result = component.GetComponentInChildren<T>(includeInactive);
			
			if (result != null)
				return result;
			
			result = component.GetComponentInParent<T>(includeInactive);
			
			return result;
		}

		public static T GetComponentAnyWhere<T>(this GameObject gameObject, bool includeInactive = false) where T : Component
		{
			T result = gameObject.GetComponent<T>();
			
			if (result != null)
				return result;
			
			result = gameObject.GetComponentInChildren<T>(includeInactive);
			
			if (result != null)
				return result;
			
			result = gameObject.GetComponentInParent<T>(includeInactive);
			
			return result;
		}
	}
}

