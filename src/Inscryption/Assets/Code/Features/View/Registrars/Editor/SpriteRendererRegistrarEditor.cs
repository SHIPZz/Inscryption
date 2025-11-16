using Code.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Code.Features.View.Registrars.Editor
{
	[CustomEditor(typeof(SpriteRendererRegistrar))]
	public class SpriteRendererRegistrarEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			
			SpriteRendererRegistrar registrar = (SpriteRendererRegistrar)target;
			
			EditorGUILayout.Space(10);
			
			if (GUILayout.Button("Find SpriteRenderer"))
			{
				SpriteRenderer spriteRenderer = registrar.GetComponentAnyWhere<SpriteRenderer>(true);
				
				if (spriteRenderer != null)
				{
					SerializedProperty prop = serializedObject.FindProperty("_spriteRenderer");
					prop.objectReferenceValue = spriteRenderer;
					serializedObject.ApplyModifiedProperties();
					
					EditorUtility.SetDirty(registrar);
					Debug.Log($"Found and assigned SpriteRenderer: {spriteRenderer.name}");
				}
				else
				{
					Debug.LogWarning("SpriteRenderer not found!");
				}
			}
		}
	}
}

