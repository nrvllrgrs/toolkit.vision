using System;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine.Vision
{
	public class VisionModeManager : ConfigurableSubsystem<VisionModeManager, VisionModeManagerConfig>
	{
		#region Fields

		private VisionMode m_activeMode;

		#endregion

		#region Events

		[SerializeField]
		public event EventHandler<VisionMode> Changed;

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
				Changed?.Invoke(this, value);
			}
		}

		#endregion
	}
}