using System;
using ToolkitEngine.VisualScripting;
using Unity.VisualScripting;

namespace ToolkitEngine.Vision.VisualScripting
{
	[UnitCategory("Events/Vision")]
	[UnitTitle("On Vision Mode Changed")]
	public class OnVisionModeChanged : FilteredTargetEventUnit<VisionMode, VisionMode>
	{
		#region Properties

		public override Type MessageListenerType => typeof(OnVisionModeChangedMessageListener);

		#endregion

		#region Methods

		protected override void StartListeningToManager()
		{
			VisionModeManager.CastInstance.Changed += InvokeTrigger;
		}

		protected override void StopListeningToManager()
		{
			VisionModeManager.CastInstance.Changed -= InvokeTrigger;
		}

		protected override VisionMode GetFilterValue(VisionMode args) => args;

		#endregion
	}
}