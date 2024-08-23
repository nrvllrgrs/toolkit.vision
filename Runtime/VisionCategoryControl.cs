using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Vision
{
	public class VisionCategoryControl : MonoBehaviour, IVisionModeEvents
    {
		#region Fields

		[SerializeField]
		private VisionCategory m_category;

		[SerializeField]
		private List<Renderer> m_ignoredRenderers = new();

		private Dictionary<Renderer, List<Material>> m_defaultMap = new();

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<VisionMode> m_onChanged;

		#endregion

		#region Properties

		public VisionCategory category => m_category;
		public UnityEvent<VisionMode> onChanged => m_onChanged;

		#endregion

		#region Methods

		private void Awake()
		{
			foreach (var renderer in GetComponentsInChildren<Renderer>())
			{
				if (m_ignoredRenderers.Contains(renderer))
					continue;

				if ((m_category.renderType & VisionCategory.RenderType.Toggle) != 0)
				{
					renderer.enabled = false;
				}

				m_defaultMap.Add(renderer, new List<Material>(renderer.materials));
			}
		}

		private void OnEnable()
		{
			VisionModeManager.Instance.onChanged.AddListener(Changed);
		}

		private void OnDisable()
		{
			if (VisionModeManager.Exists)
			{
				VisionModeManager.Instance.onChanged.RemoveListener(Changed);
			}
		}

		private void Changed(VisionMode mode)
		{
			if (m_category == null)
				return;

			if (mode != null)
			{
				SetMaterial(mode);
				EnableRenderer(true);
			}
			else
			{
				EnableRenderer(false);
				SetMaterial(null);
			}

			m_onChanged?.Invoke(mode);
		}

		private void SetMaterial(VisionMode mode)
		{
			if ((m_category.renderType & VisionCategory.RenderType.Replace) == 0)
				return;

			if (mode != null && m_category.TryGetMaterial(mode, out Material material) && material != null)
			{
				foreach (var p in m_defaultMap)
				{
					p.Key.SetMaterials(Enumerable.Repeat(material, p.Value.Count).ToList());
				}
			}
			else
			{
				foreach (var p in m_defaultMap)
				{
					p.Key.SetMaterials(p.Value);
				}
			}
		}

		private void EnableRenderer(bool enabled)
		{
			if ((m_category.renderType & VisionCategory.RenderType.Toggle) == 0)
				return;

			foreach (var p in m_defaultMap)
			{
				p.Key.enabled = enabled;
			}
		}

		#endregion
	}
}