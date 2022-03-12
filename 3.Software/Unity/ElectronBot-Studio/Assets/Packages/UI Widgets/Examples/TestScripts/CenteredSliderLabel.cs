namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// CenteredSlider with label.
	/// </summary>
	[RequireComponent(typeof(CenteredSlider))]
	public class CenteredSliderLabel : MonoBehaviour, IUpgradeable
	{
		/// <summary>
		/// Label to display slider value.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("label")]
		[HideInInspector]
		[Obsolete("Replaced with LabelAdapter.")]
		protected Text Label;

		/// <summary>
		/// Label to display slider value.
		/// </summary>
		[SerializeField]
		protected TextAdapter LabelAdapter;

		/// <summary>
		/// Current slider.
		/// </summary>
		[HideInInspector]
		[NonSerialized]
		protected CenteredSlider Slider;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init and adds listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void Init()
		{
			Slider = GetComponent<CenteredSlider>();
			if (Slider != null)
			{
				Slider.OnValueChanged.AddListener(ValueChanged);
				ValueChanged(Slider.Value);
			}
		}

		/// <summary>
		/// Callback when slider value changed.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void ValueChanged(int value)
		{
			LabelAdapter.text = value.ToString();
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (Slider != null)
			{
				Slider.OnValueChanged.RemoveListener(ValueChanged);
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Label, ref LabelAdapter);
#pragma warning restore 0612, 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif
	}
}