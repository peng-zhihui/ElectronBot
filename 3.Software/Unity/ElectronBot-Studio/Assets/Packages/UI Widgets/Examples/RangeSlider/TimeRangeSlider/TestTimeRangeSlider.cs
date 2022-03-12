namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Test TimeRangeSlider.
	/// </summary>
	[RequireComponent(typeof(TimeRangeSlider))]
	public class TestTimeRangeSlider : MonoBehaviour, IUpgradeable
	{
		/// <summary>
		/// Text component to display start time.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("startText")]
		[HideInInspector]
		[Obsolete("Replaced with StartTextAdapter.")]
		protected Text StartText;

		/// <summary>
		/// Text component to display end time.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("endText")]
		[HideInInspector]
		[Obsolete("Replaced with EndTextAdapter.")]
		protected Text EndText;

		/// <summary>
		/// Text component to display start time.
		/// </summary>
		[SerializeField]
		protected TextAdapter StartTextAdapter;

		/// <summary>
		/// Text component to display end time.
		/// </summary>
		[SerializeField]
		protected TextAdapter EndTextAdapter;

		/// <summary>
		/// Time format.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("format")]
		protected string Format = "hh:mm tt";

		/// <summary>
		/// Current slider.
		/// </summary>
		[HideInInspector]
		protected TimeRangeSlider Slider;

		/// <summary>
		/// Start this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void Start()
		{
			Slider = GetComponent<TimeRangeSlider>();
			if (Slider != null)
			{
				Slider.OnChange.AddListener(SliderChanged);

				SetRange1();

				Slider.StartTime = Slider.MinTime;
				Slider.EndTime = Slider.MaxTime;

				SliderChanged(Slider.StartTime, Slider.EndTime);
			}
		}

		/// <summary>
		/// Set slider range.
		/// </summary>
		public void SetRange1()
		{
			Slider.MinTime = new DateTime(2017, 1, 1, 9, 0, 0);
			Slider.MaxTime = new DateTime(2017, 1, 1, 18, 0, 0);

			SliderChanged(Slider.StartTime, Slider.EndTime);
		}

		/// <summary>
		/// Set another range.
		/// </summary>
		public void SetRange2()
		{
			Slider.MinTime = new DateTime(2017, 1, 1, 12, 0, 0);
			Slider.MaxTime = new DateTime(2017, 1, 1, 15, 0, 0);

			SliderChanged(Slider.StartTime, Slider.EndTime);
		}

		/// <summary>
		/// Handle slider value changed event.
		/// </summary>
		/// <param name="start">Start time.</param>
		/// <param name="end">End time.</param>
		protected virtual void SliderChanged(DateTime start, DateTime end)
		{
			if (StartTextAdapter != null)
			{
				StartTextAdapter.text = start.ToString(Format);
			}

			if (EndTextAdapter != null)
			{
				EndTextAdapter.text = end.ToString(Format);
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (Slider != null)
			{
				Slider.OnChange.RemoveListener(SliderChanged);
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(StartText, ref StartTextAdapter);
			Utilities.GetOrAddComponent(EndText, ref EndTextAdapter);
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