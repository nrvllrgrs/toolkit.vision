using System;
using System.Collections.Generic;
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

		public bool TryGetProperties(VisionMode visionMode, out IEnumerable<PropertyData> properties)
		{
			if (visionMode != null && m_map.TryGetValue(visionMode, out var data))
			{
				properties = data.properties;
				return true;
			}

			properties = null;
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
			private PropertyData[] m_properties;

			#endregion

			#region Properties

			public Material material => m_material;
			public bool enabled => m_enabled;
			public PropertyData[] properties => m_properties;

			#endregion
		}

		#endregion
	}
}