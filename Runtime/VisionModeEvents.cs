using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Vision
{
	public class VisionModeEvents : MonoBehaviour, IVisionModeEvents
	{
		#region Events

		[SerializeField]
		private UnityEvent<VisionMode> m_onChanged;

		#endregion

		#region Properties

		public UnityEvent<VisionMode> onChanged => m_onChanged;

		#endregion
	}
}