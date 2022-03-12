namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// InputField listener.
	/// </summary>
	public class InputFieldListener : SelectListener, IUpdateSelectedHandler
	{
		/// <summary>
		/// Move event.
		/// </summary>
		[SerializeField]
		public MoveEvent OnMoveEvent = new MoveEvent();

		/// <summary>
		/// Submit event.
		/// </summary>
		[SerializeField]
		public SubmitEvent OnSubmitEvent = new SubmitEvent();

		/// <summary>
		/// OnUpdateSelected event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnUpdateSelected(BaseEventData eventData)
		{
			if (CompatibilityInput.IsLeftArrowDown)
			{
				var axisEvent = new AxisEventData(EventSystem.current)
				{
					moveDir = MoveDirection.Left,
				};
				OnMoveEvent.Invoke(axisEvent);
				return;
			}

			if (CompatibilityInput.IsRightArrowDown)
			{
				var axisEvent = new AxisEventData(EventSystem.current)
				{
					moveDir = MoveDirection.Right,
				};
				OnMoveEvent.Invoke(axisEvent);
				return;
			}

			if (CompatibilityInput.IsUpArrowDown)
			{
				var axisEvent = new AxisEventData(EventSystem.current)
				{
					moveDir = MoveDirection.Up,
				};
				OnMoveEvent.Invoke(axisEvent);
				return;
			}

			if (CompatibilityInput.IsDownArrowDown)
			{
				var axisEvent = new AxisEventData(EventSystem.current)
				{
					moveDir = MoveDirection.Down,
				};
				OnMoveEvent.Invoke(axisEvent);
				return;
			}

			if (CompatibilityInput.IsTabDown || CompatibilityInput.IsEnterDown)
			{
				var isEnter = CompatibilityInput.IsEnterPressed;
				var isShift = CompatibilityInput.IsShiftPressed;
				if (!(isEnter && isShift))
				{
					OnSubmitEvent.Invoke(eventData, isEnter);
				}

				return;
			}
		}
	}
}