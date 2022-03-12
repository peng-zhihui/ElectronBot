namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Base class for Time widget.
	/// </summary>
	[DataBindSupport]
	public abstract class TimeBase : UIBehaviourConditional, IStylable
	{
		#region Interactable
		[SerializeField]
		bool interactable = true;

		/// <summary>
		/// Is widget interactable.
		/// </summary>
		/// <value><c>true</c> if interactable; otherwise, <c>false</c>.</value>
		public bool Interactable
		{
			get
			{
				return interactable;
			}

			set
			{
				if (interactable != value)
				{
					interactable = value;
					InteractableChanged();
				}
			}
		}

		/// <summary>
		/// If the canvas groups allow interaction.
		/// </summary>
		protected bool GroupsAllowInteraction = true;

		/// <summary>
		/// The CanvasGroup cache.
		/// </summary>
		protected List<CanvasGroup> CanvasGroupCache = new List<CanvasGroup>();

		/// <summary>
		/// Process the CanvasGroupChanged event.
		/// </summary>
		protected override void OnCanvasGroupChanged()
		{
			var groupAllowInteraction = true;
			var t = transform;
			while (t != null)
			{
				t.GetComponents(CanvasGroupCache);
				var shouldBreak = false;
				foreach (var canvas_group in CanvasGroupCache)
				{
					if (!canvas_group.interactable)
					{
						groupAllowInteraction = false;
						shouldBreak = true;
					}

					shouldBreak |= canvas_group.ignoreParentGroups;
				}

				if (shouldBreak)
				{
					break;
				}

				t = t.parent;
			}

			if (groupAllowInteraction != GroupsAllowInteraction)
			{
				GroupsAllowInteraction = groupAllowInteraction;
				InteractableChanged();
			}
		}

		/// <summary>
		/// Returns true if the GameObject and the Component are active.
		/// </summary>
		/// <returns>true if the GameObject and the Component are active; otherwise false.</returns>
		public override bool IsActive()
		{
			return base.IsActive() && GroupsAllowInteraction && Interactable && isInited;
		}

		/// <summary>
		/// Process interactable change.
		/// </summary>
		protected virtual void InteractableChanged()
		{
			if (!base.IsActive() || !isInited)
			{
				return;
			}

			OnInteractableChange(GroupsAllowInteraction && Interactable);
		}

		/// <summary>
		/// Process interactable change.
		/// </summary>
		/// <param name="interactableState">Current interactable state.</param>
		protected virtual void OnInteractableChange(bool interactableState)
		{
		}
		#endregion

		[SerializeField]
		bool currentTimeAsDefault = false;

		/// <summary>
		/// The time as text.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("currentTimeAsDefault", false)]
		protected string timeText = DateTime.Now.TimeOfDay.ToString();

		/// <summary>
		/// The minimum time as text.
		/// </summary>
		[SerializeField]
		protected string timeMinText = "00:00:00";

		/// <summary>
		/// The maximum time as text.
		/// </summary>
		[SerializeField]
		protected string timeMaxText = "23:59:59";

		/// <summary>
		/// The time.
		/// </summary>
		[SerializeField]
		protected TimeSpan time = DateTime.Now.TimeOfDay;

		/// <summary>
		/// The time.
		/// </summary>
		[DataBindField]
		public virtual TimeSpan Time
		{
			get
			{
				return time;
			}

			set
			{
				Init();

				if (time == value)
				{
					return;
				}

				time = value;

				if (time.Ticks < 0)
				{
					time += new TimeSpan(1, 0, 0, 0);
				}

				if (time.Days > 0)
				{
					time -= new TimeSpan(time.Days, 0, 0, 0);
				}

				time = Clamp(time);

				UpdateInputs();
				OnTimeChanged.Invoke(time);
			}
		}

		/// <summary>
		/// The minimum time.
		/// </summary>
		[SerializeField]
		protected TimeSpan timeMin = new TimeSpan(0, 0, 0);

		/// <summary>
		/// The minimum time.
		/// </summary>
		/// <value>The time minimum.</value>
		public TimeSpan TimeMin
		{
			get
			{
				return timeMin;
			}

			set
			{
				timeMin = value;
				Time = Clamp(Time);
			}
		}

		/// <summary>
		/// The maximum time.
		/// </summary>
		[SerializeField]
		protected TimeSpan timeMax = new TimeSpan(23, 59, 59);

		/// <summary>
		/// The maximum time.
		/// </summary>
		/// <value>The time max.</value>
		public TimeSpan TimeMax
		{
			get
			{
				return timeMax;
			}

			set
			{
				timeMax = value;
				Time = Clamp(Time);
			}
		}

		/// <summary>
		/// The OnTimeChanged event.
		/// </summary>
		[DataBindEvent("Time")]
		public TimeEvent OnTimeChanged = new TimeEvent();

		bool isInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			if (currentTimeAsDefault)
			{
				time = DateTime.Now.TimeOfDay;
			}
			else
			{
				if (!TimeSpan.TryParse(timeText, out time))
				{
					Debug.LogWarning("TimeText cannot be parsed.");
				}
			}

			if (!TimeSpan.TryParse(timeMinText, out timeMin))
			{
				Debug.LogWarning("TimeMinText cannot be parsed.");
			}

			if (!TimeSpan.TryParse(timeMaxText, out timeMax))
			{
				Debug.LogWarning("TimeMaxText cannot be parsed.");
			}

			InitInput();

			UpdateInputs();

			AddListeners();

			InteractableChanged();

			Time = Clamp(Time);
		}

		/// <summary>
		/// Init the input.
		/// </summary>
		protected abstract void InitInput();

		/// <summary>
		/// Clamp the specified time.
		/// </summary>
		/// <param name="t">Time.</param>
		/// <returns>Clamped time.</returns>
		protected virtual TimeSpan Clamp(TimeSpan t)
		{
			if (t < timeMin)
			{
				t = timeMin;
			}

			if (t > timeMax)
			{
				t = timeMax;
			}

			return t;
		}

		/// <summary>
		/// Add the listeners.
		/// </summary>
		protected abstract void AddListeners();

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected abstract void RemoveListeners();

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();
			RemoveListeners();
		}

		/// <summary>
		/// Updates the inputs.
		/// </summary>
		public abstract void UpdateInputs();

		#region IStylable implementation

		/// <inheritdoc/>
		public abstract bool SetStyle(Style style);

		/// <inheritdoc/>
		public abstract bool GetStyle(Style style);
		#endregion
	}
}