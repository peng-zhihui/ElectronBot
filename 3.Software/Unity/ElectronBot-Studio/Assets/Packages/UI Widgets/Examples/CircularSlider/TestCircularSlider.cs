namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// CircularSlider with label.
	/// </summary>
	[RequireComponent(typeof(CircularSlider))]
	public class TestCircularSlider : MonoBehaviour
	{
		/// <summary>
		/// Text component to display min value.
		/// </summary>
		[SerializeField]
		protected TextAdapter Label;

		CircularSlider slider;

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
			slider = GetComponent<CircularSlider>();
			if (slider != null)
			{
				slider.OnValueChanged.AddListener(ValueChanged);
				ValueChanged(slider.Value);
			}
		}

		/// <summary>
		/// Callback when slider value changed.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void ValueChanged(int value)
		{
			Label.text = value.ToString();
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (slider != null)
			{
				slider.OnValueChanged.RemoveListener(ValueChanged);
			}
		}
	}
}