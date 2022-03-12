namespace UIWidgets
{
	using System.Collections;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Switch direction.
	/// </summary>
	public enum SwitchDirection
	{
		/// <summary>
		/// Left to right.
		/// </summary>
		LeftToRight = 0,

		/// <summary>
		/// Right to left.
		/// </summary>
		RightToLeft = 1,

		/// <summary>
		/// Bottom to top.
		/// </summary>
		BottomToTop = 2,

		/// <summary>
		/// Top to bottom.
		/// </summary>
		TopToBottom = 3,
	}

	/// <summary>
	/// Switch.
	/// </summary>
	[DataBindSupport]
	public class Switch : Selectable, ISubmitHandler, IPointerClickHandler, IStylable, IValidateable
	{
		/// <summary>
		/// Is on?
		/// </summary>
		[SerializeField]
		protected bool isOn;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is on.
		/// </summary>
		/// <value><c>true</c> if this instance is on; otherwise, <c>false</c>.</value>
		[DataBindField]
		public virtual bool IsOn
		{
			get
			{
				return isOn;
			}

			set
			{
				if (isOn != value)
				{
					isOn = value;
					if (Group != null)
					{
						if (isOn || (!Group.AnySwitchesOn() && !Group.AllowSwitchOff))
						{
							isOn = true;
							Group.NotifySwitchOn(this);
						}
					}

					Changed();
				}
			}
		}

		/// <summary>
		/// Switch group.
		/// </summary>
		[SerializeField]
		protected SwitchGroup Group;

		/// <summary>
		/// Gets or sets the switch group.
		/// </summary>
		/// <value>The switch group.</value>
		public virtual SwitchGroup SwitchGroup
		{
			get
			{
				return Group;
			}

			set
			{
				if (Group != null)
				{
					Group.UnregisterSwitch(this);
				}

				Group = value;

				if (Group != null)
				{
					Group.RegisterSwitch(this);
					if (IsOn)
					{
						Group.NotifySwitchOn(this);
					}
				}
			}
		}

		/// <summary>
		/// The direction.
		/// </summary>
		[SerializeField]
		protected SwitchDirection direction = SwitchDirection.LeftToRight;

		/// <summary>
		/// Gets or sets the direction.
		/// </summary>
		/// <value>The direction.</value>
		public SwitchDirection Direction
		{
			get
			{
				return direction;
			}

			set
			{
				direction = value;
				SetMarkPosition(false);
			}
		}

		/// <summary>
		/// The mark.
		/// </summary>
		[SerializeField]
		public RectTransform Mark;

		/// <summary>
		/// The mark.
		/// </summary>
		[SerializeField]
		public Graphic MarkGraphic;

		/// <summary>
		/// The background.
		/// </summary>
		[SerializeField]
		public Graphic Background;

		[SerializeField]
		Color markOnColor = new Color(1f, 1f, 1f, 1f);

		/// <summary>
		/// Gets or sets the color of the mark for On state.
		/// </summary>
		/// <value>The color of the mark for On state.</value>
		public Color MarkOnColor
		{
			get
			{
				return markOnColor;
			}

			set
			{
				markOnColor = value;
				SetMarkColor();
			}
		}

		[SerializeField]
		Color markOffColor = new Color(1f, 215f / 255f, 115f / 255f, 1f);

		/// <summary>
		/// Gets or sets the color of the mark for Off State.
		/// </summary>
		/// <value>The color of the mark for Off State.</value>
		public Color MarkOffColor
		{
			get
			{
				return markOffColor;
			}

			set
			{
				markOffColor = value;
				SetMarkColor();
			}
		}

		[SerializeField]
		Color backgroundOnColor = new Color(1f, 1f, 1f, 1f);

		/// <summary>
		/// Gets or sets the color of the background for On state.
		/// </summary>
		/// <value>The color of the background on.</value>
		public Color BackgroundOnColor
		{
			get
			{
				return backgroundOnColor;
			}

			set
			{
				backgroundOnColor = value;
				SetBackgroundColor();
			}
		}

		[SerializeField]
		Color backgroundOffColor = new Color(1f, 215f / 255f, 115f / 255f, 1f);

		/// <summary>
		/// Gets or sets the color of the background for Off State.
		/// </summary>
		/// <value>The color of the background off.</value>
		public Color BackgroundOffColor
		{
			get
			{
				return backgroundOffColor;
			}

			set
			{
				backgroundOffColor = value;
				SetBackgroundColor();
			}
		}

		/// <summary>
		/// The duration of the animation.
		/// </summary>
		[SerializeField]
		public float AnimationDuration = 0.3f;

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime;

		/// <summary>
		/// Callback executed when the IsOn of the switch is changed.
		/// </summary>
		[SerializeField]
		[DataBindEvent("IsOn")]
		public SwitchEvent OnValueChanged = new SwitchEvent();

		/// <summary>
		/// Set IsOn without OnValueChanged invoke. Not recommended for use.
		/// </summary>
		/// <param name="value">If set to <c>true</c> value.</param>
		/// <param name="animate">Change state with animation.</param>
		public void SetStatus(bool value, bool animate = true)
		{
			if (isOn == value)
			{
				return;
			}

			isOn = value;

			if (Group != null)
			{
				if (isOn || (!Group.AnySwitchesOn() && !Group.AllowSwitchOff))
				{
					isOn = true;
					Group.NotifySwitchOn(this, false, animate);
				}
			}

			SetMarkPosition(animate);

			SetBackgroundColor();
			SetMarkColor();
		}

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			SwitchGroup = Group;
		}

		/// <summary>
		/// Changed this instance.
		/// </summary>
		protected virtual void Changed()
		{
			SetMarkPosition();

			SetBackgroundColor();
			SetMarkColor();

			OnValueChanged.Invoke(IsOn);
		}

		/// <summary>
		/// The current coroutine.
		/// </summary>
		protected IEnumerator CurrentCoroutine;

		/// <summary>
		/// Sets the mark position.
		/// </summary>
		/// <param name="animate">If set to <c>true</c> animate.</param>
		protected virtual void SetMarkPosition(bool animate = true)
		{
			if (CurrentCoroutine != null)
			{
				StopCoroutine(CurrentCoroutine);
				CurrentCoroutine = null;
			}

			if (animate && gameObject.activeInHierarchy)
			{
				CurrentCoroutine = AnimateSwitch(IsOn, AnimationDuration);
				StartCoroutine(CurrentCoroutine);
			}
			else
			{
				SetMarkPosition(GetPosition(IsOn));
			}
		}

		/// <summary>
		/// Animates the switch.
		/// </summary>
		/// <returns>The switch.</returns>
		/// <param name="state">IsOn.</param>
		/// <param name="time">Time.</param>
		protected virtual IEnumerator AnimateSwitch(bool state, float time)
		{
			SetMarkPosition(GetPosition(!IsOn));

			var prev_position = GetPosition(!IsOn);
			var next_position = GetPosition(IsOn);
			var end_time = UtilitiesTime.GetTime(UnscaledTime) + time;

			while (UtilitiesTime.GetTime(UnscaledTime) <= end_time)
			{
				var distance = 1 - ((end_time - UtilitiesTime.GetTime(UnscaledTime)) / time);
				var pos = Mathf.Lerp(prev_position, next_position, distance);

				SetMarkPosition(pos);

				yield return null;
			}

			SetMarkPosition(GetPosition(IsOn));
		}

		/// <summary>
		/// Get time.
		/// </summary>
		/// <returns>Time.</returns>
		[System.Obsolete("Use Utilities.GetTime(UnscaledTime).")]
		protected float GetTime()
		{
			return Utilities.GetTime(UnscaledTime);
		}

		/// <summary>
		/// Sets the mark position.
		/// </summary>
		/// <param name="position">Position.</param>
		protected virtual void SetMarkPosition(float position)
		{
			if (Mark == null)
			{
				return;
			}

			if (IsHorizontal())
			{
				Mark.anchorMin = new Vector2(position, Mark.anchorMin.y);
				Mark.anchorMax = new Vector2(position, Mark.anchorMax.y);
				Mark.pivot = new Vector2(position, Mark.pivot.y);
			}
			else
			{
				Mark.anchorMin = new Vector2(Mark.anchorMin.x, position);
				Mark.anchorMax = new Vector2(Mark.anchorMax.x, position);
				Mark.pivot = new Vector2(Mark.pivot.x, position);
			}
		}

		/// <summary>
		/// Determines whether this instance is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		protected bool IsHorizontal()
		{
			return Direction == SwitchDirection.LeftToRight || Direction == SwitchDirection.RightToLeft;
		}

		/// <summary>
		/// Gets the position.
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="state">State.</param>
		protected float GetPosition(bool state)
		{
			switch (Direction)
			{
				case SwitchDirection.LeftToRight:
				case SwitchDirection.BottomToTop:
					return state ? 1f : 0f;
				case SwitchDirection.RightToLeft:
				case SwitchDirection.TopToBottom:
					return state ? 0f : 1f;
			}

			return 0f;
		}

		/// <summary>
		/// Sets the color of the background.
		/// </summary>
		protected virtual void SetBackgroundColor()
		{
			if (Background == null)
			{
				return;
			}

			Background.color = IsOn ? BackgroundOnColor : BackgroundOffColor;
		}

		/// <summary>
		/// Sets the color of the mark.
		/// </summary>
		protected virtual void SetMarkColor()
		{
			if (MarkGraphic == null)
			{
				return;
			}

			MarkGraphic.color = IsOn ? MarkOnColor : MarkOffColor;
		}

		/// <summary>
		/// Calls the changed.
		/// </summary>
		protected virtual void CallChanged()
		{
			if (!IsActive() || !IsInteractable())
			{
				return;
			}

			IsOn = !IsOn;
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnSubmit event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSubmit(BaseEventData eventData)
		{
			CallChanged();
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerClick event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			CallChanged();
		}

		/// <summary>
		/// Process the dimensions changed event.
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			if (!IsActive())
			{
				return;
			}

			SetMarkPosition(false);
			SetBackgroundColor();
			SetMarkColor();
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			SetMarkPosition(false);
			SetBackgroundColor();
			SetMarkColor();
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Handle validate event.
		/// </summary>
		public virtual void Validate()
		{
			SetMarkPosition(false);
			SetBackgroundColor();
			SetMarkColor();
		}
		#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Switch.Border.ApplyTo(GetComponent<Image>());

			style.Switch.BackgroundDefault.ApplyTo(Background as Image);

			if (Mark != null)
			{
				style.Switch.MarkDefault.ApplyTo(Mark.GetComponent<Image>());
			}

			backgroundOnColor = style.Switch.BackgroundOnColor;
			backgroundOffColor = style.Switch.BackgroundOffColor;

			markOnColor = style.Switch.MarkOnColor;
			markOffColor = style.Switch.MarkOffColor;

			SetBackgroundColor();
			SetMarkColor();

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Switch.Border.GetFrom(GetComponent<Image>());

			style.Switch.BackgroundDefault.GetFrom(Background as Image);

			if (Mark != null)
			{
				style.Switch.MarkDefault.GetFrom(Mark.GetComponent<Image>());
			}

			style.Switch.BackgroundOnColor = backgroundOnColor;
			style.Switch.BackgroundOffColor = backgroundOffColor;

			style.Switch.MarkOnColor = markOnColor;
			style.Switch.MarkOffColor = markOffColor;

			return true;
		}
		#endregion
	}
}