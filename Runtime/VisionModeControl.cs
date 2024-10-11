using UnityEngine;

namespace ToolkitEngine.Vision
{
	public class VisionModeControl : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private VisionMode m_mode;

		#endregion

		#region Properties

		public VisionMode mode => m_mode;

		#endregion

		#region Methods

		[ContextMenu("Set Mode")]
		public void SetMode()
		{
			VisionModeManager.CastInstance.activeMode = mode;
		}

		[ContextMenu("Clear Mode")]
		public void ClearMode()
		{
			VisionModeManager.CastInstance.activeMode = null;
		}

		#endregion
	}
}