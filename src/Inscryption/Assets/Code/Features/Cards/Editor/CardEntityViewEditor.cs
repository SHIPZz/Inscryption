using Code.Common.Extensions;
using Code.Features.View;
using UnityEditor;
using UnityEngine;

namespace Code.Features.Cards.Editor
{
	[CustomEditor(typeof(CardEntityView))]
	public class CardEntityViewEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			
			CardEntityView cardView = (CardEntityView)target;
			
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("Auto Assign References", EditorStyles.boldLabel);
			
			EditorGUILayout.BeginHorizontal();
			
			if (GUILayout.Button("Find EntityBehaviour"))
			{
				EntityBehaviour entityBehaviour = cardView.GetComponentAnyWhere<EntityBehaviour>();
				
				if (entityBehaviour != null)
				{
					SerializedProperty prop = serializedObject.FindProperty("_entityBehaviour");
					prop.objectReferenceValue = entityBehaviour;
					serializedObject.ApplyModifiedProperties();
					
					EditorUtility.SetDirty(cardView);
					Debug.Log($"Found and assigned EntityBehaviour: {entityBehaviour.name}");
				}
				else
				{
					Debug.LogWarning("EntityBehaviour not found!");
				}
			}
			
			if (GUILayout.Button("Find SpriteRenderer"))
			{
				SpriteRenderer spriteRenderer = cardView.GetComponentAnyWhere<SpriteRenderer>(true);
				
				if (spriteRenderer != null)
				{
					SerializedProperty prop = serializedObject.FindProperty("_iconRenderer");
					prop.objectReferenceValue = spriteRenderer;
					serializedObject.ApplyModifiedProperties();
					
					EditorUtility.SetDirty(cardView);
					Debug.Log($"Found and assigned SpriteRenderer: {spriteRenderer.name}");
				}
				else
				{
					Debug.LogWarning("SpriteRenderer not found!");
				}
			}
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space(5);
			
			if (GUILayout.Button("Find All References", GUILayout.Height(30)))
			{
				bool foundAny = false;
				
				EntityBehaviour entityBehaviour = cardView.GetComponentAnyWhere<EntityBehaviour>();
				if (entityBehaviour != null)
				{
					SerializedProperty prop = serializedObject.FindProperty("_entityBehaviour");
					prop.objectReferenceValue = entityBehaviour;
					foundAny = true;
					Debug.Log($"Found EntityBehaviour: {entityBehaviour.name}");
				}
				
				SpriteRenderer spriteRenderer = cardView.GetComponentAnyWhere<SpriteRenderer>(true);
				if (spriteRenderer != null)
				{
					SerializedProperty prop = serializedObject.FindProperty("_iconRenderer");
					prop.objectReferenceValue = spriteRenderer;
					foundAny = true;
					Debug.Log($"Found SpriteRenderer: {spriteRenderer.name}");
				}
				
				if (foundAny)
				{
					serializedObject.ApplyModifiedProperties();
					EditorUtility.SetDirty(cardView);
					Debug.Log("All available references assigned!");
				}
				else
				{
					Debug.LogWarning("No references found!");
				}
			}
		}
	}
}

