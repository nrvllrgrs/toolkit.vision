using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Vision.VisualScripting
{
	[AddComponentMenu("")]
	public class OnVisionModeChangedMessageListener : MessageListener
	{
		private void Start() => GetComponent<IVisionModeEvents>()?.onChanged.AddListener((value) =>
		{
			EventBus.Trigger(nameof(OnVisionModeChanged), gameObject, value);
		});
	}
}
