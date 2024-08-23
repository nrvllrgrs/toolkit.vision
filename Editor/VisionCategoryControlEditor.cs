using UnityEditor;
using ToolkitEngine.Vision;

namespace ToolkitEditor.Vision
{
	[CustomEditor(typeof(VisionCategoryControl))]
    public class VisionCategoryControlEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_category;
		protected SerializedProperty m_ignoredRenderers;
		protected SerializedProperty m_onChanged;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_category = serializedObject.FindProperty(nameof(m_category));
			m_ignoredRenderers = serializedObject.FindProperty(nameof (m_ignoredRenderers));
			m_onChanged = serializedObject.FindProperty(nameof (m_onChanged));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_category);
			EditorGUILayout.PropertyField(m_ignoredRenderers);
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onChanged, "Events"))
			{
				EditorGUILayout.PropertyField(m_onChanged);
			}
		}

		#endregion
	}
}