namespace UIWidgets
{
	using System;
	using System.Collections;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Base class for Time widget.
	/// </summary>
	[DataBindSupport]
	public class TimeSpinnerBase : TimeBase, IUpgradeable
	{
		/// <summary>
		/// The input field for the hours.
		/// </summary>
		[SerializeField]
		protected InputFieldAdapter InputHoursAdapter;

		/// <summary>
		/// The input field for the minutes.
		/// </summary>
		[SerializeField]
		protected InputFieldAdapter InputMinutesAdapter;

		/// <summary>
		/// The input field for the seconds.
		/// </summary>
		[SerializeField]
		protected InputFieldAdapter InputSecondsAdapter;

		/// <summary>
		/// The input field proxy for the hours.
		/// </summary>
		[Obsolete("Replaced with InputHoursAdapter.")]
		protected IInputFieldProxy InputProxyHours
		{
			get
			{
				return InputHoursAdapter;
			}
		}

		/// <summary>
		/// The input field proxy for the minutes.
		/// </summary>
		[Obsolete("Replaced with InputMinutesAdapter.")]
		protected IInputFieldProxy InputProxyMinutes
		{
			get
			{
				return InputMinutesAdapter;
			}
		}

		/// <summary>
		/// The input field proxy for the seconds.
		/// </summary>
		[Obsolete("Replaced with InputSecondsAdapter.")]
		protected IInputFieldProxy InputProxySeconds
		{
			get
			{
				return InputSecondsAdapter;
			}
		}

		/// <summary>
		/// The button to increase hours.
		/// </summary>
		[SerializeField]
		protected ButtonAdvanced ButtonHoursIncrease;

		/// <summary>
		/// The button to decrease hours.
		/// </summary>
		[SerializeField]
		protected ButtonAdvanced ButtonHoursDecrease;

		/// <summary>
		/// The button to increase minutes.
		/// </summary>
		[SerializeField]
		protected ButtonAdvanced ButtonMinutesIncrease;

		/// <summary>
		/// The button to decrease minutes.
		/// </summary>
		[SerializeField]
		protected ButtonAdvanced ButtonMinutesDecrease;

		/// <summary>
		/// The button to increase seconds.
		/// </summary>
		[SerializeField]
		protected ButtonAdvanced ButtonSecondsIncrease;

		/// <summary>
		/// The button to decrease seconds.
		/// </summary>
		[SerializeField]
		protected ButtonAdvanced ButtonSecondsDecrease;

		/// <summary>
		/// Allow changing value during hold.
		/// </summary>
		[SerializeField]
		public bool AllowHold = true;

		/// <summary>
		/// Delay of hold in seconds for permanent increase/decrease value.
		/// </summary>
		[SerializeField]
		public float HoldStartDelay = 0.5f;

		/// <summary>
		/// Delay of hold in seconds between increase/decrease value.
		/// </summary>
		[SerializeField]
		public float HoldChangeDelay = 0.1f;

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		IEnumerator HoldCoroutine;

		/// <summary>
		/// Init the input.
		/// </summary>
		protected override void InitInput()
		{
			Upgrade();
		}

		/// <summary>
		/// Process interactable change.
		/// </summary>
		/// <param name="interactableState">Current interactable state.</param>
		protected override void OnInteractableChange(bool interactableState)
		{
			if (InputHoursAdapter != null)
			{
				InputHoursAdapter.interactable = interactableState;
			}

			if (InputMinutesAdapter != null)
			{
				InputMinutesAdapter.interactable = interactableState;
			}

			if (InputSecondsAdapter != null)
			{
				InputSecondsAdapter.interactable = interactableState;
			}

			if (ButtonHoursIncrease != null)
			{
				ButtonHoursIncrease.interactable = interactableState;
			}

			if (ButtonHoursDecrease != null)
			{
				ButtonHoursDecrease.interactable = interactableState;
			}

			if (ButtonMinutesIncrease != null)
			{
				ButtonMinutesIncrease.interactable = interactableState;
			}

			if (ButtonMinutesDecrease != null)
			{
				ButtonMinutesDecrease.interactable = interactableState;
			}

			if (ButtonSecondsIncrease != null)
			{
				ButtonSecondsIncrease.interactable = interactableState;
			}

			if (ButtonSecondsDecrease != null)
			{
				ButtonSecondsDecrease.interactable = interactableState;
			}
		}

		/// <summary>
		/// Add the listeners.
		/// </summary>
		protected override void AddListeners()
		{
			if (InputHoursAdapter != null)
			{
				InputHoursAdapter.onEndEdit.AddListener(UpdateHours);
			}

			if (InputMinutesAdapter != null)
			{
				InputMinutesAdapter.onEndEdit.AddListener(UpdateMinutes);
			}

			if (InputSecondsAdapter != null)
			{
				InputSecondsAdapter.onEndEdit.AddListener(UpdateSeconds);
			}

			if (ButtonHoursIncrease != null)
			{
				ButtonHoursIncrease.onClick.AddListener(HoursIncrease);
				ButtonHoursIncrease.onPointerDown.AddListener(ButtonHoursIncreaseDown);
				ButtonHoursIncrease.onPointerUp.AddListener(ButtonUp);
			}

			if (ButtonHoursDecrease != null)
			{
				ButtonHoursDecrease.onClick.AddListener(HoursDecrease);
				ButtonHoursDecrease.onPointerDown.AddListener(ButtonHoursDecreaseDown);
				ButtonHoursDecrease.onPointerUp.AddListener(ButtonUp);
			}

			if (ButtonMinutesIncrease != null)
			{
				ButtonMinutesIncrease.onClick.AddListener(MinutesIncrease);
				ButtonMinutesIncrease.onPointerDown.AddListener(ButtonMinutesIncreaseDown);
				ButtonMinutesIncrease.onPointerUp.AddListener(ButtonUp);
			}

			if (ButtonMinutesDecrease != null)
			{
				ButtonMinutesDecrease.onClick.AddListener(MinutesDecrease);
				ButtonMinutesDecrease.onPointerDown.AddListener(ButtonMinutesDecreaseDown);
				ButtonMinutesDecrease.onPointerUp.AddListener(ButtonUp);
			}

			if (ButtonSecondsIncrease != null)
			{
				ButtonSecondsIncrease.onClick.AddListener(SecondsIncrease);
				ButtonSecondsIncrease.onPointerDown.AddListener(ButtonSecondsIncreaseDown);
				ButtonSecondsIncrease.onPointerUp.AddListener(ButtonUp);
			}

			if (ButtonSecondsDecrease != null)
			{
				ButtonSecondsDecrease.onClick.AddListener(SecondsDecrease);
				ButtonSecondsDecrease.onPointerDown.AddListener(ButtonSecondsDecreaseDown);
				ButtonSecondsDecrease.onPointerUp.AddListener(ButtonUp);
			}
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected override void RemoveListeners()
		{
			if (InputHoursAdapter != null)
			{
				InputHoursAdapter.onEndEdit.RemoveListener(UpdateHours);
			}

			if (InputMinutesAdapter != null)
			{
				InputMinutesAdapter.onEndEdit.RemoveListener(UpdateMinutes);
			}

			if (InputSecondsAdapter != null)
			{
				InputSecondsAdapter.onEndEdit.RemoveListener(UpdateSeconds);
			}

			if (ButtonHoursIncrease != null)
			{
				ButtonHoursIncrease.onClick.RemoveListener(HoursIncrease);
				ButtonHoursIncrease.onPointerDown.RemoveListener(ButtonHoursIncreaseDown);
				ButtonHoursIncrease.onPointerUp.RemoveListener(ButtonUp);
			}

			if (ButtonHoursDecrease != null)
			{
				ButtonHoursDecrease.onClick.RemoveListener(HoursDecrease);
				ButtonHoursDecrease.onPointerDown.RemoveListener(ButtonHoursDecreaseDown);
				ButtonHoursDecrease.onPointerUp.RemoveListener(ButtonUp);
			}

			if (ButtonMinutesIncrease != null)
			{
				ButtonMinutesIncrease.onClick.RemoveListener(MinutesIncrease);
				ButtonMinutesIncrease.onPointerDown.RemoveListener(ButtonMinutesIncreaseDown);
				ButtonMinutesIncrease.onPointerUp.RemoveListener(ButtonUp);
			}

			if (ButtonMinutesDecrease != null)
			{
				ButtonMinutesDecrease.onClick.RemoveListener(MinutesDecrease);
				ButtonMinutesDecrease.onPointerDown.RemoveListener(ButtonMinutesDecreaseDown);
				ButtonMinutesDecrease.onPointerUp.RemoveListener(ButtonUp);
			}

			if (ButtonSecondsIncrease != null)
			{
				ButtonSecondsIncrease.onClick.RemoveListener(SecondsIncrease);
				ButtonSecondsIncrease.onPointerDown.RemoveListener(ButtonSecondsIncreaseDown);
				ButtonSecondsIncrease.onPointerUp.RemoveListener(ButtonUp);
			}

			if (ButtonSecondsDecrease != null)
			{
				ButtonSecondsDecrease.onClick.RemoveListener(SecondsDecrease);
				ButtonSecondsDecrease.onPointerDown.RemoveListener(ButtonSecondsDecreaseDown);
				ButtonSecondsDecrease.onPointerUp.RemoveListener(ButtonUp);
			}
		}

		/// <summary>
		/// Updates the hours.
		/// </summary>
		/// <param name="hours">Hours.</param>
		protected virtual void UpdateHours(string hours)
		{
			int h;
			if (int.TryParse(hours, out h))
			{
				Time += new TimeSpan(Mathf.Abs(h) - time.Hours, 0, 0);
			}
			else
			{
				UpdateInputs();
			}
		}

		/// <summary>
		/// Updates the minutes.
		/// </summary>
		/// <param name="minutes">Minutes.</param>
		protected virtual void UpdateMinutes(string minutes)
		{
			int m;
			if (int.TryParse(minutes, out m))
			{
				Time += new TimeSpan(0, Mathf.Abs(m) - time.Minutes, 0);
			}
			else
			{
				UpdateInputs();
			}
		}

		/// <summary>
		/// Updates the seconds.
		/// </summary>
		/// <param name="seconds">Seconds.</param>
		protected virtual void UpdateSeconds(string seconds)
		{
			int s;
			if (int.TryParse(seconds, out s))
			{
				Time += new TimeSpan(0, 0, Mathf.Abs(s) - time.Seconds);
			}
			else
			{
				UpdateInputs();
			}
		}

		/// <summary>
		/// Increase the hours.
		/// </summary>
		public void HoursIncrease()
		{
			Time += new TimeSpan(1, 0, 0);
		}

		/// <summary>
		/// Decrease the hours.
		/// </summary>
		public void HoursDecrease()
		{
			Time -= new TimeSpan(1, 0, 0);
		}

		/// <summary>
		/// Increase the minutes.
		/// </summary>
		public void MinutesIncrease()
		{
			Time += new TimeSpan(0, 1, 0);
		}

		/// <summary>
		/// Decrease the minutes.
		/// </summary>
		public void MinutesDecrease()
		{
			Time -= new TimeSpan(0, 1, 0);
		}

		/// <summary>
		/// Increase the seconds.
		/// </summary>
		public void SecondsIncrease()
		{
			Time += new TimeSpan(0, 0, 1);
		}

		/// <summary>
		/// Decrease the seconds.
		/// </summary>
		public void SecondsDecrease()
		{
			Time -= new TimeSpan(0, 0, 1);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			RemoveListeners();
		}

		/// <summary>
		/// Updates the inputs.
		/// </summary>
		public override void UpdateInputs()
		{
			if (InputHoursAdapter != null)
			{
				InputHoursAdapter.text = Time.Hours.ToString("D2");
			}

			if (InputMinutesAdapter != null)
			{
				InputMinutesAdapter.text = Time.Minutes.ToString("D2");
			}

			if (InputSecondsAdapter != null)
			{
				InputSecondsAdapter.text = Time.Seconds.ToString("D2");
			}
		}

		#region Hold

		/// <summary>
		/// Create hold coroutine.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <returns>Hold coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
		protected virtual IEnumerator HoldCreateCoroutine(Action action)
		{
			if (AllowHold)
			{
				yield return UtilitiesTime.Wait(HoldStartDelay, UnscaledTime);
				while (AllowHold)
				{
					action();
					yield return UtilitiesTime.Wait(HoldChangeDelay, UnscaledTime);
				}
			}
		}

		/// <summary>
		/// Start hold process.
		/// </summary>
		/// <param name="action">Action.</param>
		protected virtual void HoldStart(Action action)
		{
			HoldStop();
			HoldCoroutine = HoldCreateCoroutine(action);
			StartCoroutine(HoldCoroutine);
		}

		/// <summary>
		/// Stop hold process.
		/// </summary>
		protected virtual void HoldStop()
		{
			if (HoldCoroutine != null)
			{
				StopCoroutine(HoldCoroutine);
			}
		}

		/// <summary>
		/// Process pointer up event on all buttons.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void ButtonUp(PointerEventData eventData)
		{
			HoldStop();
		}

		/// <summary>
		/// Process pointer down event on hours increase button.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void ButtonHoursIncreaseDown(PointerEventData eventData)
		{
			HoldStart(HoursIncrease);
		}

		/// <summary>
		/// Process pointer down event on hours decrease button.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void ButtonHoursDecreaseDown(PointerEventData eventData)
		{
			HoldStart(HoursDecrease);
		}

		/// <summary>
		/// Process pointer down event on minutes increase button.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void ButtonMinutesIncreaseDown(PointerEventData eventData)
		{
			HoldStart(MinutesIncrease);
		}

		/// <summary>
		/// Process pointer down event on minutes decrease button.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void ButtonMinutesDecreaseDown(PointerEventData eventData)
		{
			HoldStart(MinutesDecrease);
		}

		/// <summary>
		/// Process pointer down event on seconds increase button.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void ButtonSecondsIncreaseDown(PointerEventData eventData)
		{
			HoldStart(SecondsIncrease);
		}

		/// <summary>
		/// Process pointer down event on seconds decrease button.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void ButtonSecondsDecreaseDown(PointerEventData eventData)
		{
			HoldStart(SecondsDecrease);
		}
		#endregion

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected override void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			if (InputHoursAdapter != null)
			{
				style.Time.InputBackground.ApplyTo(InputHoursAdapter);

				if (InputHoursAdapter.textComponent != null)
				{
					style.Time.InputText.ApplyTo(InputHoursAdapter.textComponent.gameObject);
				}
			}

			if (InputMinutesAdapter != null)
			{
				style.Time.InputBackground.ApplyTo(InputMinutesAdapter);

				if (InputMinutesAdapter.textComponent != null)
				{
					style.Time.InputText.ApplyTo(InputMinutesAdapter.textComponent.gameObject);
				}
			}

			if (InputSecondsAdapter != null)
			{
				style.Time.InputBackground.ApplyTo(InputSecondsAdapter);

				if (InputSecondsAdapter.textComponent != null)
				{
					style.Time.InputText.ApplyTo(InputSecondsAdapter.textComponent.gameObject);
				}
			}

			style.Time.ButtonIncrease.ApplyTo(ButtonHoursIncrease);
			style.Time.ButtonDecrease.ApplyTo(ButtonHoursDecrease);

			style.Time.ButtonIncrease.ApplyTo(ButtonMinutesIncrease);
			style.Time.ButtonDecrease.ApplyTo(ButtonMinutesDecrease);

			style.Time.ButtonIncrease.ApplyTo(ButtonSecondsIncrease);
			style.Time.ButtonDecrease.ApplyTo(ButtonSecondsDecrease);

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			if (InputHoursAdapter != null)
			{
				style.Time.InputBackground.GetFrom(InputHoursAdapter);

				if (InputHoursAdapter.textComponent != null)
				{
					style.Time.InputText.GetFrom(InputHoursAdapter.textComponent.gameObject);
				}
			}

			if (InputMinutesAdapter != null)
			{
				style.Time.InputBackground.GetFrom(InputMinutesAdapter);

				if (InputMinutesAdapter.textComponent != null)
				{
					style.Time.InputText.GetFrom(InputMinutesAdapter.textComponent.gameObject);
				}
			}

			if (InputSecondsAdapter != null)
			{
				style.Time.InputBackground.GetFrom(InputSecondsAdapter);

				if (InputSecondsAdapter.textComponent != null)
				{
					style.Time.InputText.GetFrom(InputSecondsAdapter.textComponent.gameObject);
				}
			}

			style.Time.ButtonIncrease.GetFrom(ButtonHoursIncrease);
			style.Time.ButtonDecrease.GetFrom(ButtonHoursDecrease);

			style.Time.ButtonIncrease.GetFrom(ButtonMinutesIncrease);
			style.Time.ButtonDecrease.GetFrom(ButtonMinutesDecrease);

			style.Time.ButtonIncrease.GetFrom(ButtonSecondsIncrease);
			style.Time.ButtonDecrease.GetFrom(ButtonSecondsDecrease);

			return true;
		}
		#endregion
	}
}