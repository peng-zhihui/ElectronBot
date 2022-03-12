namespace UIWidgets
{
	using System;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	/// <summary>
	/// Scroll buttons.
	/// </summary>
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollButtons : MonoBehaviour
	{
		/// <summary>
		/// Is the ScrollButtons eligible for interaction?
		/// </summary>
		[SerializeField]
		public bool Interactable = true;

		/// <summary>
		/// ScrollRect.
		/// </summary>
		protected ScrollRect scrollRect;

		/// <summary>
		/// ScrollRect.
		/// </summary>
		public ScrollRect ScrollRect
		{
			get
			{
				if (scrollRect == null)
				{
					scrollRect = GetComponent<ScrollRect>();
				}

				return scrollRect;
			}
		}

		/// <summary>
		/// Left scroll button.
		/// </summary>
		[SerializeField]
		protected RectTransform scrollButtonLeft;

		/// <summary>
		/// Gets or sets the left scroll button.
		/// </summary>
		/// <value>The scroll button left.</value>
		public RectTransform ScrollButtonLeft
		{
			get
			{
				return scrollButtonLeft;
			}

			set
			{
				scrollButtonLeft = SetScrollButton(scrollButtonLeft, value, ScrollOnHoldLeft, ScrollOnClickLeft);
			}
		}

		/// <summary>
		/// Right scroll button.
		/// </summary>
		[SerializeField]
		protected RectTransform scrollButtonRight;

		/// <summary>
		/// Gets or sets the right scroll button.
		/// </summary>
		/// <value>The scroll button right.</value>
		public RectTransform ScrollButtonRight
		{
			get
			{
				return scrollButtonRight;
			}

			set
			{
				scrollButtonRight = SetScrollButton(scrollButtonRight, value, ScrollOnHoldRight, ScrollOnClickRight);
			}
		}

		/// <summary>
		/// Top scroll button.
		/// </summary>
		[SerializeField]
		protected RectTransform scrollButtonTop;

		/// <summary>
		/// Gets or sets the top scroll button.
		/// </summary>
		/// <value>The scroll button top.</value>
		public RectTransform ScrollButtonTop
		{
			get
			{
				return scrollButtonTop;
			}

			set
			{
				scrollButtonTop = SetScrollButton(scrollButtonTop, value, ScrollOnHoldTop, ScrollOnClickTop);
			}
		}

		/// <summary>
		/// Bottom scroll button.
		/// </summary>
		[SerializeField]
		protected RectTransform scrollButtonBottom;

		/// <summary>
		/// Gets or sets the bottom scroll button.
		/// </summary>
		/// <value>The scroll button bottom.</value>
		public RectTransform ScrollButtonBottom
		{
			get
			{
				return scrollButtonBottom;
			}

			set
			{
				scrollButtonBottom = SetScrollButton(scrollButtonBottom, value, ScrollOnHoldBottom, ScrollOnClickBottom);
			}
		}

		/// <summary>
		/// Scroll sensitivity rate on click.
		/// Scroll on click on ScrollRect.scrollSensitivity * Rate
		/// </summary>
		[SerializeField]
		public float ScrollSensitivityRateOnClick = 10f;

		/// <summary>
		/// Scroll sensitivity rate on hold.
		/// Scroll during hold on ScrollRect.scrollSensitivity * Rate per 1 second
		/// </summary>
		[SerializeField]
		public float ScrollSensitivityRateHold = 50f;

		/// <summary>
		/// Animate scroll on click or scroll immediately?
		/// </summary>
		[SerializeField]
		[Tooltip("Animate scroll on click or scroll immediately?")]
		public bool Animate = true;

		/// <summary>
		/// Animation curve on click.
		/// </summary>
		[SerializeField]
		protected AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 0.2f, 1f);

		/// <summary>
		/// The length of the animation.
		/// </summary>
		protected float AnimationLength;

		/// <summary>
		/// Curve for click animation.
		/// </summary>
		/// <value>The curve.</value>
		public AnimationCurve Curve
		{
			get
			{
				return curve;
			}

			set
			{
				curve = value;
				AnimationLength = curve[curve.length - 1].time;
			}
		}

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime;

		/// <summary>
		/// Current animation coroutine.
		/// </summary>
		protected IEnumerator CurrentCoroutine;

		/// <summary>
		/// Animation start time.
		/// </summary>
		protected float AnimationStartTime;

		/// <summary>
		/// ScrollRect start position.
		/// </summary>
		protected Vector3 StartPosition;

		bool isInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
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

			Curve = curve;

			scrollButtonLeft = SetScrollButton(scrollButtonLeft, scrollButtonLeft, ScrollOnHoldLeft, ScrollOnClickLeft);
			scrollButtonRight = SetScrollButton(scrollButtonRight, scrollButtonRight, ScrollOnHoldRight, ScrollOnClickRight);
			scrollButtonTop = SetScrollButton(scrollButtonTop, scrollButtonTop, ScrollOnHoldTop, ScrollOnClickTop);
			scrollButtonBottom = SetScrollButton(scrollButtonBottom, scrollButtonBottom, ScrollOnHoldBottom, ScrollOnClickBottom);

			isInited = true;
		}

		/// <summary>
		/// Scroll on hold left button.
		/// </summary>
		protected virtual void ScrollOnHoldLeft()
		{
			ScrolOnHold(ScrollButtonType.Left);
		}

		/// <summary>
		/// Scroll on hold right button.
		/// </summary>
		protected virtual void ScrollOnHoldRight()
		{
			ScrolOnHold(ScrollButtonType.Right);
		}

		/// <summary>
		/// Scroll on hold top button.
		/// </summary>
		protected virtual void ScrollOnHoldTop()
		{
			ScrolOnHold(ScrollButtonType.Top);
		}

		/// <summary>
		/// Scroll on hold bottom button.
		/// </summary>
		protected virtual void ScrollOnHoldBottom()
		{
			ScrolOnHold(ScrollButtonType.Bottom);
		}

		/// <summary>
		/// Scroll on hold.
		/// </summary>
		/// <param name="type">Type.</param>
		public void ScrolOnHold(ScrollButtonType type)
		{
			if (!Interactable)
			{
				return;
			}

			AnimationStop();

			AnimationStartTime = UtilitiesTime.GetTime(UnscaledTime);
			StartPosition = ScrollRect.content.localPosition;

			ScrollRect.StopMovement();

			CurrentCoroutine = AnimationHold(type);
			StartCoroutine(CurrentCoroutine);
		}

		/// <summary>
		/// Stop animation.
		/// </summary>
		protected virtual void AnimationStop()
		{
			if (CurrentCoroutine != null)
			{
				StopCoroutine(CurrentCoroutine);
			}
		}

		/// <summary>
		/// Sets the scroll button.
		/// </summary>
		/// <param name="oldButton">Old button.</param>
		/// <param name="newButton">New button.</param>
		/// <param name="downAction">Action on pointer down.</param>
		/// <param name="clickAction">Action on pointer click.</param>
		/// <returns>Returns new button.</returns>
		protected virtual RectTransform SetScrollButton(RectTransform oldButton, RectTransform newButton, UnityAction downAction, UnityAction clickAction)
		{
			if (oldButton != null)
			{
				Utilities.GetOrAddComponent<ScrollButton>(oldButton).OnDown.RemoveListener(downAction);
				Utilities.GetOrAddComponent<ScrollButton>(oldButton).OnUp.RemoveListener(AnimationStop);
				Utilities.GetOrAddComponent<ScrollButton>(oldButton).OnClick.RemoveListener(clickAction);
			}

			if (newButton != null)
			{
				Utilities.GetOrAddComponent<ScrollButton>(newButton).OnDown.AddListener(downAction);
				Utilities.GetOrAddComponent<ScrollButton>(newButton).OnUp.AddListener(AnimationStop);
				Utilities.GetOrAddComponent<ScrollButton>(newButton).OnClick.AddListener(clickAction);
			}

			return newButton;
		}

		/// <summary>
		/// Scroll on click left.
		/// </summary>
		public void ScrollOnClickLeft()
		{
			ScrollOnClick(ScrollButtonType.Left);
		}

		/// <summary>
		/// Scroll on click right.
		/// </summary>
		public void ScrollOnClickRight()
		{
			ScrollOnClick(ScrollButtonType.Right);
		}

		/// <summary>
		/// Scroll on click top.
		/// </summary>
		public void ScrollOnClickTop()
		{
			ScrollOnClick(ScrollButtonType.Top);
		}

		/// <summary>
		/// Scroll on click bottom.
		/// </summary>
		public void ScrollOnClickBottom()
		{
			ScrollOnClick(ScrollButtonType.Bottom);
		}

		/// <summary>
		/// Scroll on click.
		/// </summary>
		/// <param name="type">Type.</param>
		public void ScrollOnClick(ScrollButtonType type)
		{
			if (!Interactable)
			{
				return;
			}

			AnimationStop();

			if ((AnimationStartTime + AnimationLength) < UtilitiesTime.GetTime(UnscaledTime))
			{
				return;
			}

			ScrollRect.StopMovement();
			var start = ScrollRect.content.localPosition;
			var end = ScrollOnClick(type, start, ScrollRect.scrollSensitivity * ScrollSensitivityRateOnClick);
			if (Animate)
			{
				CurrentCoroutine = AnimationClick(start, end);
				StartCoroutine(CurrentCoroutine);
			}
			else
			{
				ScrollRect.content.localPosition = end;
				ForceUpdateBounds();
			}
		}

		/// <summary>
		/// Force ScrollRect to update bounds.
		/// </summary>
		protected void ForceUpdateBounds()
		{
			if (ScrollRect.movementType != ScrollRect.MovementType.Unrestricted)
			{
				ScrollRect.horizontalNormalizedPosition = Mathf.Clamp(ScrollRect.horizontalNormalizedPosition, 0f, 1f);
				ScrollRect.verticalNormalizedPosition = Mathf.Clamp(ScrollRect.verticalNormalizedPosition, 0f, 1f);
			}
		}

		/// <summary>
		/// Animation on hold.
		/// </summary>
		/// <returns>Nothing.</returns>
		/// <param name="type">Type.</param>
		protected virtual IEnumerator AnimationHold(ScrollButtonType type)
		{
			do
			{
				var delta = UtilitiesTime.GetDeltaTime(UnscaledTime);
				var step = scrollRect.scrollSensitivity * ScrollSensitivityRateHold * delta;
				ScrollRect.content.localPosition = ScrollOnClick(type, ScrollRect.content.localPosition, step);
				ForceUpdateBounds();

				yield return null;
			}
			while (gameObject.activeSelf);
		}

		/// <summary>
		/// Gets the time.
		/// </summary>
		/// <returns>The time.</returns>
		[Obsolete("Use UtilitiesTime.GetTime(UnscaledTime).")]
		protected virtual float GetTime()
		{
			return UtilitiesTime.GetTime(UnscaledTime);
		}

		/// <summary>
		/// Animation on click.
		/// </summary>
		/// <returns>Nothing.</returns>
		/// <param name="start">Start scroll position.</param>
		/// <param name="end">End scroll position.</param>
		protected virtual IEnumerator AnimationClick(Vector3 start, Vector3 end)
		{
			var animation_position = 0f;
			do
			{
				animation_position = UtilitiesTime.GetTime(UnscaledTime) - AnimationStartTime;
				var pos = Vector3.Lerp(start, end, Curve.Evaluate(animation_position));

				ScrollRect.content.localPosition = pos;
				ForceUpdateBounds();

				yield return null;
			}
			while (animation_position < AnimationLength);

			ScrollRect.content.localPosition = end;
			ForceUpdateBounds();
		}

		/// <summary>
		/// Get scroll.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="position">Position.</param>
		/// <param name="value">Scroll value.</param>
		/// <returns>New position.</returns>
		protected virtual Vector3 ScrollOnClick(ScrollButtonType type, Vector3 position, float value)
		{
			switch (type)
			{
				case ScrollButtonType.Top:
					position.y -= value;
					break;
				case ScrollButtonType.Bottom:
					position.y += value;
					break;
				case ScrollButtonType.Left:
					position.x += value;
					break;
				case ScrollButtonType.Right:
					position.x -= value;
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown ScrollButtonType: {0}", EnumHelper<ScrollButtonType>.ToString(type)));
			}

			return position;
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			ScrollButtonLeft = null;
			ScrollButtonRight = null;
			ScrollButtonTop = null;
			ScrollButtonBottom = null;
		}
	}
}