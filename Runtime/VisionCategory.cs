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
			Property = 1 << 2,
		};

		#endregion

		#region Fields

		[SerializeField]
		private RenderType m_renderType = RenderType.Replace;

		[SerializeField]
		private VisionModeDataMap m_map = new();

		#endregion

		#region Properties

		public RenderType renderType => m_renderType;

		#endregion

		#region Methods

		public bool TryGetMaterial(VisionMode visionMode, out Material material)
		{
			if (visionMode != null && m_map.TryGetValue(visionMode, out var data))
			{
				material = data.material;
				return true;
			}

			material = null;
			return false;
		}

		public bool TryGetPropertyNameAndValue(VisionMode visionMode, out string name, out int value)
		{
			if (visionMode != null && m_map.TryGetValue(visionMode, out var data))
			{
				name = data.propertyName;
				value = data.propertyValue;
				return true;
			}

			name = null;
			value = 0;
			return false;
		}

		public bool GetEnabled(VisionMode visionMode)
		{
			return visionMode != null && m_map.TryGetValue(visionMode, out var data)
				? data.enabled
				: false;
		}

		#endregion

		#region Structures

		[Serializable]
		public class VisionModeDataMap : SerializableDictionary<VisionMode, VisionModeData>
		{ }

		[Serializable]
		public class VisionModeData
		{
			#region Fields

			[SerializeField]
			private Material m_material;

			[SerializeField]
			private bool m_enabled = true;

			[SerializeField]
			private string m_propertyName;

			[SerializeField]
			private int m_propertyValue;

			#endregion

			#region Properties

			public Material material => m_material;
			public bool enabled => m_enabled;
			public string propertyName => m_propertyName;
			public int propertyValue => m_propertyValue;

			#endregion
		}

		#endregion
	}
}