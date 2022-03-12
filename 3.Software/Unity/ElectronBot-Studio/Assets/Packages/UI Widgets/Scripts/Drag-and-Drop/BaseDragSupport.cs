namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Used only to attach custom editor to DragSupport.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public abstract class BaseDragSupport : UIBehaviour
	{
		/// <summary>
		/// The drag points.
		/// </summary>
		protected static Dictionary<int, RectTransform> DragPoints = new Dictionary<int, RectTransform>();

		RectTransform parentCanvas;

		/// <summary>
		/// Gets a canvas transform of current gameobject.
		/// </summary>
		protected RectTransform ParentCanvas
		{
			get
			{
				if (parentCanvas == null)
				{
					parentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
				}

				return parentCanvas;
			}
		}

		/// <summary>
		/// Gets the drag point.
		/// </summary>
		public RectTransform DragPoint
		{
			get
			{
				var contains_key = DragPoints.ContainsKey(ParentCanvas.GetInstanceID());
				if (!contains_key || (DragPoints[ParentCanvas.GetInstanceID()] == null))
				{
					var go = new GameObject("DragPoint");
					var dragPoint = go.AddComponent<RectTransform>();
					dragPoint.SetParent(ParentCanvas, false);
					dragPoint.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
					dragPoint.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);

					DragPoints[ParentCanvas.GetInstanceID()] = dragPoint;
				}

				return DragPoints[ParentCanvas.GetInstanceID()];
			}
		}
	}
}