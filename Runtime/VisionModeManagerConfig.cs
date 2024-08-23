using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine.Vision
{
	[CreateAssetMenu(menuName = "Toolkit/Vision/Vision Mode Config")]
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
