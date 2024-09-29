using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine.Vision
{
	[CreateAssetMenu(menuName = "Toolkit/Config/VisionModeManager Config")]
	public class VisionModeManagerConfig : ScriptableObject
	{
		#region Fields

		[SerializeField]
		private List<VisionMode> m_modes;

		#endregion

		#region Properties

		public VisionMode[] modes => m_modes.Where(x => x != null).ToArray();

		#endregion
	}
}
