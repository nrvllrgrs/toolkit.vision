using UnityEngine;
using UnityEditor;
using ToolkitEngine.Vision;
using static ToolkitEngine.Vision.PropertyData;

namespace ToolkitEditor.Vision
{
	[CustomPropertyDrawer(typeof(PropertyData))]
    public class PropertyDataDrawer : PropertyDrawer
    {
		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty typeProp = property.FindPropertyRelative("m_type");
			EditorGUIRectLayout.PropertyField(ref position, typeProp);

			var typeValue = (PropertyType)typeProp.intValue;
			if (typeValue != PropertyType.Preset)
			{
				EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_name"));
			}

			SerializedProperty valueProp;
			switch (typeValue)
			{
				case PropertyType.Boolean:
				case PropertyType.Keyword:
					valueProp = property.FindPropertyRelative("m_boolValue");
					valueProp.boolValue = EditorGUIRectLayout.ToggleField(ref position, "Value", valueProp.boolValue);
					break;

				case PropertyType.Color:
					valueProp = property.FindPropertyRelative("m_colorValue");
					valueProp.colorValue = EditorGUIRectLayout.ColorField(ref position, "Value", valueProp.colorValue);
					break;

				case PropertyType.Float:
					valueProp = property.FindPropertyRelative("m_floatValue");
					valueProp.floatValue = EditorGUIRectLayout.FloatField(ref position, "Value", valueProp.floatValue);
					break;

				case PropertyType.Integer:
					valueProp = property.FindPropertyRelative("m_intValue");
					valueProp.intValue = EditorGUIRectLayout.IntField(ref position, "Value", valueProp.intValue);
					break;

				case PropertyType.Preset:
					valueProp = property.FindPropertyRelative("m_preset");
					EditorGUIRectLayout.PropertyField(ref position, valueProp, new GUIContent("Value"));
					break;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_name"))
				+ EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_type"))
				+ (EditorGUIUtility.standardVerticalSpacing * 2f);

			var typeValue = (PropertyType)property.FindPropertyRelative("m_type").intValue;
			if (typeValue != PropertyType.Preset)
			{
				height += EditorGUIUtility.singleLineHeight
					+ EditorGUIUtility.standardVerticalSpacing;
			}

			return height;
		}

		#endregion
	}
}
