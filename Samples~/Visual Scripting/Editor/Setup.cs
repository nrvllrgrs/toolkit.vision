using System;
using System.Collections.Generic;
using ToolkitEngine.Vision;
using UnityEditor;

namespace ToolkitEditor.Vision.VisualScripting
{
	[InitializeOnLoad]
	public static class Setup
	{
		static Setup()
		{
			var types = new List<Type>()
			{
				typeof(VisionModeManager),
				typeof(VisionMode),
				typeof(VisionModeControl),
				typeof(VisionCategory),
				typeof(VisionCategoryControl),
			};

			ToolkitEditor.VisualScripting.Setup.Initialize("ToolkitEngine.Vision", types);
		}
	}
}