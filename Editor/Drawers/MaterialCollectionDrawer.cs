using UnityEngine;
using UnityEditor;
using ToolkitEngine.Vision;

namespace ToolkitEditor.Vision
{
    [CustomPropertyDrawer(typeof(MaterialCollection))]
    public class MaterialCollectionDrawer : PropertyDrawer
    {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_materials"), label);
        }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_materials"), label);
		}
	}
}