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
		protected SerializedProperty m_map;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_renderType = serializedObject.FindProperty(nameof(m_renderType));
			m_map = serializedObject.FindProperty(nameof(m_map));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_renderType);

			var keysProp = m_map.FindPropertyRelative("keys");
			var valuesProp = m_map.FindPropertyRelative("values");

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

			for (int i = 0; i < keysProp.arraySize; ++i)
			{
				var prop = valuesProp.GetArrayElementAtIndex(i);
				var renderTypeValue = (VisionCategory.RenderType)m_renderType.intValue;

				if (EditorGUILayoutUtility.Foldout(prop, keysProp.GetArrayElementAtIndex(i).objectReferenceValue.name))
				{
					++EditorGUI.indentLevel;

					if ((renderTypeValue & VisionCategory.RenderType.Replace) != 0)
					{
						EditorGUILayout.PropertyField(prop.FindPropertyRelative("m_material"));
					}

					if ((renderTypeValue & VisionCategory.RenderType.Toggle) != 0)
					{
						EditorGUILayout.PropertyField(prop.FindPropertyRelative("m_enabled"));
					}

					if ((renderTypeValue & VisionCategory.RenderType.Property) != 0)
					{
						EditorGUILayout.LabelField("Property");

						++EditorGUI.indentLevel;
						EditorGUILayout.PropertyField(prop.FindPropertyRelative("m_propertyName"), new GUIContent("Name"));
						EditorGUILayout.PropertyField(prop.FindPropertyRelative("m_propertyValue"), new GUIContent("Value"));
						--EditorGUI.indentLevel;
					}

					--EditorGUI.indentLevel;
				}
			}
		}

		#endregion
	}
}