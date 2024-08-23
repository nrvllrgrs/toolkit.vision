using Unity.VisualScripting;

namespace ToolkitEngine.Vision.VisualScripting
{
	[UnitCategory("Vision")]
	[UnitTitle("Clear Vision Mode")]
	public class ClearVisionMode : Unit
	{
		#region Fields

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter;

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit;

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), Enter);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);
		}

		private ControlOutput Enter(Flow flow)
		{
			VisionModeManager.Instance.activeMode = null;
			return exit;
		}

		#endregion
	}
}