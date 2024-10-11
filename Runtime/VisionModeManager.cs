using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Vision
{
	public class VisionModeManager : ConfigurableSubsystem<VisionModeManager, VisionModeManagerConfig>, IVisionModeEvents
	{
		#region Fields

		private VisionMode m_activeMode;

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<VisionMode> m_onChanged;

		#endregion

		#region Properties

		public VisionMode activeMode
		{
			get => m_activeMode;
			set
			{
				// No change, skip
				if (m_activeMode == value)
					return;

				// Non-null value does not exist in config, skip
				if (value != null && !Config.modes.Contains(value))
					return;

				m_activeMode = value;
				m_onChanged?.Invoke(value);
			}
		}

		public UnityEvent<VisionMode> onChanged => m_onChanged;

		#endregion
	}
}