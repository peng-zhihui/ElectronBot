namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Range slider handle.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class RangeSliderHandle : Selectable, IDragHandler, IInitializePotentialDragHandler, ISubmitHandler
	{
		/// <summary>
		/// Is horizontal direction?
		/// </summary>
		public Func<bool> IsHorizontal = ReturnTrue;

		/// <summary>
		/// Decrease on move.
		/// </summary>
		public Action Decrease = DoNothing;

		/// <summary>
		/// Increase on move.
		/// </summary>
		public Action Increase = DoNothing;

		/// <summary>
		/// The position limits.
		/// </summary>
		public Func<Vector2> PositionLimits;

		/// <summary>
		/// On position changed.
		/// </summary>
		public Action<float> PositionChanged;

		/// <summary>
		/// On submit.
		/// </summary>
		public Action OnSubmit = DoNothing;

		RectTransform rectTransform;

		/// <summary>
		/// Gets or sets the rect transform.
		/// </summary>
		/// <value>The rect transform.</value>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}

			set
			{
				rectTransform = value;
			}
		}

		/// <summary>
		/// Determines whether this instance can drag the specified eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can drag the specified eventData; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Event data.</param>
		bool CanDrag(PointerEventData eventData)
		{
			return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
		}

		static bool ReturnTrue()
		{
			return true;
		}

		static void DoNothing()
		{
		}

		/// <summary>
		/// Process the submit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		void ISubmitHandler.OnSubmit(BaseEventData eventData)
		{
			OnSubmit();
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}

			Vector2 position;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out position);

			PositionChanged(IsHorizontal() ? position.x : position.y);
		}

		/// <summary>
		/// Process the initialize potential drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
		}

		/// <summary>
		/// Process the move event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnMove(AxisEventData eventData)
		{
			if (!IsActive() || !IsInteractable())
			{
				base.OnMove(eventData);
				return;
			}

			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
					if (IsHorizontal() && FindSelectableOnLeft() == null)
					{
						Decrease();
					}
					else
					{
						base.OnMove(eventData);
					}

					break;
				case MoveDirection.Right:
					if (IsHorizontal() && FindSelectableOnRight() == null)
					{
						Increase();
					}
					else
					{
						base.OnMove(eventData);
					}

					break;
				case MoveDirection.Up:
					if (!IsHorizontal() && FindSelectableOnUp() == null)
					{
						Increase();
					}
					else
					{
						base.OnMove(eventData);
					}

					break;
				case MoveDirection.Down:
					if (!IsHorizontal() && FindSelectableOnDown() == null)
					{
						Decrease();
					}
					else
					{
						base.OnMove(eventData);
					}

					break;
			}
		}

		/// <summary>
		/// Finds the selectable on left.
		/// </summary>
		/// <returns>The selectable on left.</returns>
		public override Selectable FindSelectableOnLeft()
		{
			if (navigation.mode == Navigation.Mode.Automatic && IsHorizontal())
			{
				return null;
			}

			return base.FindSelectableOnLeft();
		}

		/// <summary>
		/// Finds the selectable on right.
		/// </summary>
		/// <returns>The selectable on right.</returns>
		public override Selectable FindSelectableOnRight()
		{
			if (navigation.mode == Navigation.Mode.Automatic && IsHorizontal())
			{
				return null;
			}

			return base.FindSelectableOnRight();
		}

		/// <summary>
		/// Finds the selectable on up.
		/// </summary>
		/// <returns>The selectable on up.</returns>
		public override Selectable FindSelectableOnUp()
		{
			if (navigation.mode == Navigation.Mode.Automatic && !IsHorizontal())
			{
				return null;
			}

			return base.FindSelectableOnUp();
		}

		/// <summary>
		/// Finds the selectable on down.
		/// </summary>
		/// <returns>The selectable on down.</returns>
		public override Selectable FindSelectableOnDown()
		{
			if (navigation.mode == Navigation.Mode.Automatic && !IsHorizontal())
			{
				return null;
			}

			return base.FindSelectableOnDown();
		}
	}
}