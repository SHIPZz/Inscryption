using Code.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Code.Features.View.Registrars.Editor
{
	[CustomEditor(typeof(EntityComponentRegistrar), true)]
	public class EntityComponentRegistrarEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			
			EntityComponentRegistrar registrar = (EntityComponentRegistrar)target;
			
			EditorGUILayout.Space(10);
			
			if (GUILayout.Button("Find EntityBehaviour"))
			{
				EntityBehaviour entityBehaviour = registrar.GetComponentAnyWhere<EntityBehaviour>();
				
				if (entityBehaviour != null)
				{
					SerializedProperty prop = serializedObject.FindProperty("EntityView");
					prop.objectReferenceValue = entityBehaviour;
					serializedObject.ApplyModifiedProperties();
					
					EditorUtility.SetDirty(registrar);
					Debug.Log($"Found and assigned EntityBehaviour: {entityBehaviour.name}");
				}
				else
				{
					Debug.LogWarning("EntityBehaviour not found!");
				}
			}
		}
	}
}

