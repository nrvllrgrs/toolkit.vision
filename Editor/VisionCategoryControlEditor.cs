using UnityEditor;
using ToolkitEngine.Vision;

namespace ToolkitEditor.Vision
{
	[CustomEditor(typeof(VisionCategoryControl))]
    public class VisionCategoryControlEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_category;
		protected SerializedProperty m_target;
		protected SerializedProperty m_ignoredRenderers;
		protected SerializedProperty m_ignoredMaterials;
		protected SerializedProperty m_onChanged;
		protected SerializedProperty m_onNormalized;
		protected SerializedProperty m_onSpecialized;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_category = serializedObject.FindProperty(nameof(m_category));
			m_target = serializedObject.FindProperty(nameof(m_target));
			m_ignoredRenderers = serializedObject.FindProperty(nameof (m_ignoredRenderers));
			m_ignoredMaterials = serializedObject.FindProperty(nameof(m_ignoredMaterials));
			m_onChanged = serializedObject.FindProperty(nameof (m_onChanged));
			m_onNormalized = serializedObject.FindProperty(nameof(m_onNormalized));
			m_onSpecialized = serializedObject.FindProperty(nameof(m_onSpecialized));
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(m_category);
			EditorGUILayout.PropertyField(m_target);
			EditorGUILayout.PropertyField(m_ignoredRenderers);
			EditorGUILayout.PropertyField(m_ignoredMaterials);
		}

		protected override void DrawEvents()
		{
			if (EditorGUILayoutUtility.Foldout(m_onChanged, "Events"))
			{
				EditorGUILayout.PropertyField(m_onChanged);
				EditorGUILayout.PropertyField(m_onNormalized);
				EditorGUILayout.PropertyField(m_onSpecialized);
			}
		}

		#endregion
	}
}
