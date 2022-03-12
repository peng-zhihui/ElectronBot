namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// RectTransform Drag.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class DragRectTransform : DragSupport<RectTransform>
	{
		/// <summary>
		/// Restore position on end drag if the snap is not applicable.
		/// </summary>
		[SerializeField]
		[Tooltip("Restore position if the drop canceled.")]
		public bool RestorePositionIfCanceled = false;

		/// <summary>
		/// Start position.
		/// </summary>
		protected Vector3 StartPosition;

		/// <inheritdoc/>
		protected override void InitDrag(PointerEventData eventData)
		{
			Data = transform as RectTransform;
			StartPosition = Data.localPosition;
		}

		/// <inheritdoc/>
		public override void Dropped(bool success)
		{
			if (!success && RestorePositionIfCanceled && (Data != null))
			{
				Data.localPosition = StartPosition;
			}
		}
	}
}