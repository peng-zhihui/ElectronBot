namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Analog Time widget.
	/// </summary>
	public class TimeAnalog : TimeBase
	{
		/// <summary>
		/// The button to increase hours.
		/// </summary>
		[SerializeField]
		protected CircularSlider Slider;

		/// <summary>
		/// Step in minutes.
		/// </summary>
		[SerializeField]
		[Tooltip("Step in minutes.")]
		[Range(1, 60)]
		protected int step = 5;

		/// <summary>
		/// Step in minutes.
		/// </summary>
		public int Step
		{
			get
			{
				Init();
				return (Slider != null) ? Slider.Step : step;
			}

			set
			{
				step = value;
				Slider.Step = value;
			}
		}

		/// <summary>
		/// The AMPM button.
		/// </summary>
		[SerializeField]
		protected Button AMPMButton;

		/// <summary>
		/// The AMPM text.
		/// </summary>
		[SerializeField]
		protected TextAdapter ampmText;

		/// <summary>
		/// The AMPM text.
		/// </summary>
		public TextAdapter AMPMTextAdapter
		{
			get
			{
				return ampmText;
			}

			set
			{
				ampmText = value;
				UpdateInputs();
			}
		}

		/// <summary>
		/// Hours labels.
		/// </summary>
		[SerializeField]
		public List<GameObject> HoursLabels = new List<GameObject>();

		/// <summary>
		/// Adds the listeners.
		/// </summary>
		protected override void AddListeners()
		{
			if (AMPMButton != null)
			{
				AMPMButton.onClick.AddListener(ToggleAMPM);
			}

			if (Slider != null)
			{
				Slider.OnChange.AddListener(SliderChanged);
			}
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected override void RemoveListeners()
		{
			if (AMPMButton != null)
			{
				AMPMButton.onClick.RemoveListener(ToggleAMPM);
			}

			if (Slider != null)
			{
				Slider.OnChange.RemoveListener(SliderChanged);
			}
		}

		/// <summary>
		/// Process slider changed event.
		/// </summary>
		protected virtual void SliderChanged()
		{
			if (Slider != null)
			{
				var h = Mathf.FloorToInt(Slider.Value / 60);
				var m = Mathf.FloorToInt(Slider.Value - (h * 60));

				if (Time.Hours >= 12)
				{
					h += 12;
				}

				Time += new TimeSpan(h - Time.Hours, m - Time.Minutes, 0);
			}
		}

		/// <summary>
		/// Convert time to the slider value.
		/// </summary>
		/// <param name="time">Time.</param>
		/// <returns>Slider value.</returns>
		protected int Time2SliderValue(TimeSpan time)
		{
			var hours = time.Hours;
			if (hours >= 12)
			{
				hours -= 12;
			}

			return (hours * 60) + time.Minutes;
		}

		/// <summary>
		/// Toggles the AMPM.
		/// </summary>
		public virtual void ToggleAMPM()
		{
			Time += new TimeSpan(12, 0, 0);
		}

		/// <summary>
		/// Updates the inputs.
		/// </summary>
		public override void UpdateInputs()
		{
			if (Slider != null)
			{
				Slider.Value = Time2SliderValue(Time);
			}

			if (AMPMTextAdapter != null)
			{
				AMPMTextAdapter.text = Time.Hours < 12 ? "AM" : "PM";
			}
		}

		/// <summary>
		/// Init the input.
		/// </summary>
		protected override void InitInput()
		{
			if (Slider != null)
			{
				Slider.StartAngle = 270;
				Slider.Step = step;
				Slider.MinValue = 0;
				Slider.MaxValue = 12 * 60;
			}
		}

		/// <summary>
		/// Process interactable change.
		/// </summary>
		/// <param name="interactableState">Current interactable state.</param>
		protected override void OnInteractableChange(bool interactableState)
		{
			if (Slider != null)
			{
				Slider.Interactable = interactableState;
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			RemoveListeners();
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			// TODO time labels
			foreach (var label in HoursLabels)
			{
				style.Time.HourLabel.ApplyTo(label);
			}

			if (AMPMButton != null)
			{
				style.Time.AMPMBackground.ApplyTo(AMPMButton);
			}

			if (AMPMTextAdapter != null)
			{
				style.Time.AMPMText.ApplyTo(AMPMTextAdapter.gameObject);
			}

			if (Slider != null)
			{
				Slider.SetStyle(style);
			}

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			foreach (var label in HoursLabels)
			{
				style.Time.HourLabel.GetFrom(label);
			}

			if (AMPMButton != null)
			{
				style.Time.AMPMBackground.GetFrom(AMPMButton);
			}

			if (AMPMTextAdapter != null)
			{
				style.Time.AMPMText.GetFrom(AMPMTextAdapter.gameObject);
			}

			if (Slider != null)
			{
				Slider.GetStyle(style);
			}

			return true;
		}
		#endregion
	}
}