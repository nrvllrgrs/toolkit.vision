using UnityEngine.Events;

namespace ToolkitEngine.Vision
{
	public interface IVisionModeEvents
    {
        UnityEvent<VisionMode> onChanged { get; }
    }
}