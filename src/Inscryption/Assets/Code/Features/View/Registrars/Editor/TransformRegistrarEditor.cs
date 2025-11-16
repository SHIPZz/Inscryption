using Code.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Code.Features.View.Registrars.Editor
{
	[CustomEditor(typeof(TransformRegistrar))]
	public class TransformRegistrarEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			
			TransformRegistrar registrar = (TransformRegistrar)target;
			
			EditorGUILayout.Space(10);
			
			if (GUILayout.Button("Assign Self Transform"))
			{
				SerializedProperty prop = serializedObject.FindProperty("_targetTransform");
				prop.objectReferenceValue = registrar.transform;
				serializedObject.ApplyModifiedProperties();
				
				EditorUtility.SetDirty(registrar);
				Debug.Log($"Assigned self Transform: {registrar.name}");
			}
			
			EditorGUILayout.Space(5);
			
			if (GUILayout.Button("Find Transform in Children"))
			{
				if (registrar.transform.childCount > 0)
				{
					Transform firstChild = registrar.transform.GetChild(0);
					
					SerializedProperty prop = serializedObject.FindProperty("_targetTransform");
					prop.objectReferenceValue = firstChild;
					serializedObject.ApplyModifiedProperties();
					
					EditorUtility.SetDirty(registrar);
					Debug.Log($"Assigned first child Transform: {firstChild.name}");
				}
				else
				{
					Debug.LogWarning("No children found!");
				}
			}
		}
	}
}

