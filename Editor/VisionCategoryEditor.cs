using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using ToolkitEngine.Vision;

namespace ToolkitEditor.Vision
{
	[CustomEditor(typeof(VisionCategory))]
    public class VisionCategoryEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_renderType;
		protected SerializedProperty m_materials;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_renderType = serializedObject.FindProperty(nameof(m_renderType));
			m_materials = serializedObject.FindProperty(nameof(m_materials));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_renderType);

			if (EditorGUILayoutUtility.Foldout(m_materials, m_materials.displayName))
			{
				var keysProp = m_materials.FindPropertyRelative("keys");
				var valuesProp = m_materials.FindPropertyRelative("values");

				// Get list of vision modes
				var modes = VisionModeManager.Instance?.config.modes;

				List<VisionMode> keys = new();

				// Remove invalid vision mode
				for (int i = keysProp.arraySize - 1; i >= 0; --i)
				{
					VisionMode key = keysProp.GetArrayElementAtIndex(i).objectReferenceValue as VisionMode;
					keys.Add(key);

					if (!modes.Contains(key))
					{
						keysProp.DeleteArrayElementAtIndex(i);
						valuesProp.DeleteArrayElementAtIndex(i);
					}
				}

				// Add missing vision mode
				var missingKeys = modes.Except(keys).ToList();
				foreach (var key in missingKeys)
				{
					valuesProp.InsertArrayElementAtIndex(valuesProp.arraySize);
					keysProp.InsertArrayElementAtIndex(keysProp.arraySize);
					keysProp.GetArrayElementAtIndex(keysProp.arraySize - 1).objectReferenceValue = key;
				}

				++EditorGUI.indentLevel;

				for (int i = 0; i < keysProp.arraySize; ++i)
				{
					EditorGUILayout.PropertyField(
						valuesProp.GetArrayElementAtIndex(i),
						new GUIContent(keysProp.GetArrayElementAtIndex(i).objectReferenceValue.name));
				}

				--EditorGUI.indentLevel;
			}
		}

		#endregion
	}
}