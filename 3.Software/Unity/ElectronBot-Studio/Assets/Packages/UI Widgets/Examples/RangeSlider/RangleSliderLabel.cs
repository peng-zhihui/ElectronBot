namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// RangeSlider with label.
	/// </summary>
	[RequireComponent(typeof(RangeSlider))]
	public class RangleSliderLabel : MonoBehaviour, IUpgradeable
	{
		/// <summary>
		/// Text component to display min value.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("minLabel")]
		[HideInInspector]
		[System.Obsolete("Replaced with MinLabelAdapter.")]
		protected Text MinLabel;

		/// <summary>
		/// Text component to display max value.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("maxLabel")]
		[HideInInspector]
		[System.Obsolete("Replaced with MaxLabelAdapter.")]
		protected Text MaxLabel;

		/// <summary>
		/// Text component to display min value.
		/// </summary>
		[SerializeField]
		protected TextAdapter MinLabelAdapter;

		/// <summary>
		/// Text component to display max value.
		/// </summary>
		[SerializeField]
		protected TextAdapter MaxLabelAdapter;

		RangeSlider slider;

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
			slider = GetComponent<RangeSlider>();
			if (slider != null)
			{
				slider.OnValuesChanged.AddListener(ValueChanged);
				ValueChanged(slider.ValueMin, slider.ValueMax);
			}
		}

		/// <summary>
		/// Callback when slider value changed.
		/// </summary>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		protected virtual void ValueChanged(int min, int max)
		{
			MinLabelAdapter.text = min.ToString();
			MaxLabelAdapter.text = max.ToString();
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (slider != null)
			{
				slider.OnValuesChanged.RemoveListener(ValueChanged);
			}
		}

		/// <summary>
			/// Upgrade this instance.
			/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(MinLabel, ref MinLabelAdapter);
			Utilities.GetOrAddComponent(MaxLabel, ref MaxLabelAdapter);
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