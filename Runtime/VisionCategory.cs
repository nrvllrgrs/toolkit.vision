using System;
using UnityEngine;

namespace ToolkitEngine.Vision
{
	[CreateAssetMenu(menuName = "Toolkit/Vision/Vision Category")]
	public class VisionCategory : ScriptableObject
    {
		#region Enumerators

		[Flags]
		public enum RenderType
		{
			Replace = 1 << 0,
			Toggle = 1 << 1,
		};

		#endregion

		#region Fields

		[SerializeField]
		private RenderType m_renderType = RenderType.Replace;

		[SerializeField]
		private VisionModeMap m_materials = new();

		#endregion

		#region Properties

		public RenderType renderType => m_renderType;

		#endregion

		#region Methods

		public bool TryGetMaterial(VisionMode visionMode, out Material material)
		{
			return m_materials.TryGetValue(visionMode, out material);
		}

		#endregion

		#region Structures

		[Serializable]
		public class VisionModeMap : SerializableDictionary<VisionMode, Material>
		{ }
		
		#endregion
	}
}