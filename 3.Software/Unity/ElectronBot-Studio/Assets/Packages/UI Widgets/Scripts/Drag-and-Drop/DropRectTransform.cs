namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// RectTransform drop component.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class DropRectTransform : MonoBehaviour, IDropSupport<RectTransform>, ISnapGridSupport
	{
		[SerializeField]
		List<SnapGridBase> snapGrids;

		/// <summary>
		/// SnapGrids.
		/// </summary>
		public List<SnapGridBase> SnapGrids
		{
			get
			{
				return snapGrids;
			}

			set
			{
				snapGrids = value;
			}
		}

		[SerializeField]
		Vector2 snapDistance = new Vector2(10f, 10f);

		/// <summary>
		/// Snap distance.
		/// </summary>
		public Vector2 SnapDistance
		{
			get
			{
				return snapDistance;
			}

			set
			{
				snapDistance = value;
			}
		}

		/// <summary>
		/// Allow drop only if only snap is applicable.
		/// </summary>
		[Tooltip("Allow drop only if only snap is applicable.")]
		public bool RequireSnap = false;

		/// <summary>
		/// Is drop allowed?
		/// Accept target RectTransform and desired local position.
		/// </summary>
		public Func<RectTransform, Vector2, bool> AllowDrop;

		/// <inheritdoc/>
		public void Drop(RectTransform data, PointerEventData eventData)
		{
			data.SetParent(transform, false);

			if (SnapGrids != null)
			{
				var snap = SnapGridBase.Snap(SnapGrids, SnapDistance, data);
				data.localPosition += new Vector3(snap.Delta.x, snap.Delta.y, 0);
			}
		}

		/// <inheritdoc/>
		public bool CanReceiveDrop(RectTransform data, PointerEventData eventData)
		{
			if (data.parent == transform)
			{
				return false;
			}

			var position = data.localPosition;
			if (SnapGrids != null)
			{
				var snap = SnapGridBase.Snap(SnapGrids, snapDistance, data);
				if (RequireSnap && !snap.Snapped)
				{
					return false;
				}

				data.localPosition += new Vector3(snap.Delta.x, snap.Delta.y, 0);
			}

			if (AllowDrop != null)
			{
				if (!AllowDrop(data, position))
				{
					return false;
				}
			}

			data.localPosition = position;

			return true;
		}

		/// <inheritdoc/>
		public void DropCanceled(RectTransform data, PointerEventData eventData)
		{
		}
	}
}