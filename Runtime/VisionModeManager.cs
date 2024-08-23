using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Vision
{
	[InitializeOnLoad]
	public class VisionModeManager : Singleton<VisionModeManager>, IVisionModeEvents
	{
		#region Fields

		[SerializeField]
		private VisionModeManagerConfig m_config;

		private VisionMode m_activeMode;

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<VisionMode> m_onChanged;

		#endregion

		#region Properties

		public VisionModeManagerConfig config => m_config;

		public VisionMode activeMode
		{
			get => m_activeMode;
			set
			{
				// No change, skip
				if (m_activeMode == value)
					return;

				// Non-null value does not exist in config, skip
				if (value != null && !m_config.modes.Contains(value))
					return;

				m_activeMode = value;
				m_onChanged?.Invoke(value);
			}
		}

		public UnityEvent<VisionMode> onChanged => m_onChanged;

		#endregion
	}
}