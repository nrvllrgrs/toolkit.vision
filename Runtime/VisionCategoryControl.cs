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

		[SerializeField]
		private List<Material> m_ignoredMaterials = new();

		private Dictionary<Renderer, List<Material>> m_defaultMap = new();
		private string m_modifiedPropertyName = null;

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<VisionMode> m_onChanged;

		[SerializeField]
		private UnityEvent m_onNormalized;

		[SerializeField]
		private UnityEvent m_onSpecialized;

		#endregion

		#region Properties

		public VisionCategory category => m_category;
		public UnityEvent<VisionMode> onChanged => m_onChanged;
		public UnityEvent onNormalized => m_onNormalized;
		public UnityEvent onSpecialized => m_onSpecialized;

		#endregion

		#region Methods

		private void Awake()
		{
			foreach (var renderer in GetComponentsInChildren<Renderer>(true))
			{
				if (m_ignoredRenderers.Contains(renderer))
					continue;

				if ((m_category.renderType & VisionCategory.RenderType.Toggle) != 0)
				{
					renderer.enabled = false;
				}

				List<Material> materials = new();
				foreach (var material in renderer.materials)
				{
					if (m_ignoredMaterials.Contains(material))
						continue;

					materials.Add(material);
				}

				m_defaultMap.Add(renderer, materials);
			}
		}

		private void OnEnable()
		{
			VisionModeManager.CastInstance.Changed += Changed;
			Changed(null, VisionModeManager.CastInstance.activeMode);
		}

		private void OnDisable()
		{
			VisionModeManager.CastInstance.Changed -= Changed;
		}

		private void Changed(object sender, VisionMode mode)
		{
			if (m_category == null)
				return;

			bool overridden = false;
			if (mode != null)
			{
				overridden |= SetMaterial(mode);
				overridden |= SetMaterialProperty(mode);
				overridden |= EnableRenderer(mode, true);
			}
			else
			{
				overridden |= EnableRenderer(null, false);
				overridden |= SetMaterialProperty(null);
				overridden |= SetMaterial(null);
			}

			m_onChanged?.Invoke(mode);
			if (overridden)
			{
				m_onSpecialized?.Invoke();
			}
			else
			{
				m_onNormalized?.Invoke();
			}
		}

		private bool SetMaterial(VisionMode mode)
		{
			if ((m_category.renderType & VisionCategory.RenderType.Replace) == 0)
				return false;

			if (mode != null && m_category.TryGetMaterial(mode, out Material material) && material != null)
			{
				foreach (var p in m_defaultMap)
				{
					p.Key.SetMaterials(Enumerable.Repeat(material, p.Value.Count).ToList());
				}
				return true;
			}
			else
			{
				foreach (var p in m_defaultMap)
				{
					p.Key.SetMaterials(p.Value);
				}
				return false;
			}
		}

		private bool SetMaterialProperty(VisionMode mode)
		{
			if ((m_category.renderType & VisionCategory.RenderType.Property) == 0)
				return false;

			if (mode != null && m_category.TryGetPropertyNameAndValue(mode, out string name, out int value) && !string.IsNullOrWhiteSpace(name))
			{
				foreach (var materials in m_defaultMap.Values)
				{
					foreach (var material in materials)
					{
						material.SetInt(name, value);
					}
				}
				m_modifiedPropertyName = name;
				return true;
			}
			else if (!string.IsNullOrEmpty(m_modifiedPropertyName))
			{
				foreach (var materials in m_defaultMap.Values)
				{
					foreach (var material in materials)
					{
						material.SetInt(m_modifiedPropertyName, 0);
					}
				}
				m_modifiedPropertyName = null;
			}

			return false;
		}

		private bool EnableRenderer(VisionMode mode, bool enabled)
		{
			if ((m_category.renderType & VisionCategory.RenderType.Toggle) == 0)
				return false;

			enabled |= m_category.GetEnabled(mode);
			foreach (var p in m_defaultMap)
			{
				p.Key.enabled = enabled;
			}
			return enabled;
		}

		#endregion
	}
}