using Code.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Code.Features.View.Editor
{
	[CustomEditor(typeof(EntityDependant), true)]
	public class EntityDependantEditor : UnityEditor.Editor
	{
		private SerializedProperty _entityViewProperty;

		private void OnEnable()
		{
			_entityViewProperty = serializedObject.FindProperty("EntityView");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_entityViewProperty);

			EditorGUILayout.Space();

			if (GUILayout.Button("Find EntityBehaviour"))
			{
				var entityDependant = (EntityDependant)target;
				var entityBehaviour = ((Component)entityDependant).GetComponentAnyWhere<EntityBehaviour>();
				
				if (entityBehaviour != null)
				{
					_entityViewProperty.objectReferenceValue = entityBehaviour;
					serializedObject.ApplyModifiedProperties();
					EditorUtility.SetDirty(target);
				}
				else
				{
					Debug.LogWarning($"EntityBehaviour not found on {entityDependant.gameObject.name} or its children/parents.");
				}
			}

			EditorGUILayout.Space();
			DrawPropertiesExcluding(serializedObject, "EntityView");

			serializedObject.ApplyModifiedProperties();
		}
	}
}

