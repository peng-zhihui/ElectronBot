namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Snap grid detector
	/// </summary>
	[RequireComponent(typeof(ISnapGridSupport))]
	public class SnapGridDetector : MonoBehaviour,
		IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		/// <summary>
		/// Modes.
		/// </summary>
		public enum Modes
		{
			/// <summary>
			/// Replace.
			/// </summary>
			Replace = 0,

			/// <summary>
			/// Add.
			/// </summary>
			Add = 1,
		}

		/// <summary>
		/// Mode.
		/// </summary>
		[SerializeField]
		public Modes Mode = Modes.Replace;

		/// <summary>
		/// Raycast results.
		/// </summary>
		[NonSerialized]
		protected readonly List<RaycastResult> RaycastResults = new List<RaycastResult>();

		ISnapGridSupport[] snapGridSupport;

		/// <summary>
		/// SnapGrid support.
		/// </summary>
		public ISnapGridSupport[] SnapGridSupport
		{
			get
			{
				if (snapGridSupport == null)
				{
					snapGridSupport = GetComponents<ISnapGridSupport>();
				}

				return snapGridSupport;
			}
		}

		/// <summary>
		/// If this object is dragged?
		/// </summary>
		protected bool IsDragged;

		/// <summary>
		/// Process OnInitializePotentialDrag event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Process OnBeginDrag event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			IsDragged = true;

			ProcessDrag(eventData);
		}

		/// <summary>
		/// Process OnDrag event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!IsDragged)
			{
				return;
			}

			ProcessDrag(eventData);
		}

		/// <summary>
		/// Process OnEndDrag event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (!IsDragged)
			{
				return;
			}

			ProcessDrag(eventData);

			IsDragged = false;
		}

		/// <summary>
		/// Process drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void ProcessDrag(PointerEventData eventData)
		{
			RaycastResults.Clear();
			EventSystem.current.RaycastAll(eventData, RaycastResults);

			var grid = FindSnapGrid(eventData, RaycastResults);
			if (grid != null)
			{
				foreach (var s in SnapGridSupport)
				{
					switch (Mode)
					{
						case Modes.Add:
							Add(s, grid);
							break;
						case Modes.Replace:
							Replace(s, grid);
							break;
						default:
							throw new NotSupportedException(string.Format("Unknown mode: {0}", EnumHelper<Modes>.ToString(Mode)));
					}
				}
			}
		}

		/// <summary>
		/// Add SnapGrid.
		/// </summary>
		/// <param name="snapGridSupport">SnapGrid support component.</param>
		/// <param name="snapGrid">Snap grid to add.</param>
		protected virtual void Add(ISnapGridSupport snapGridSupport, SnapGridBase snapGrid)
		{
			if (snapGridSupport.SnapGrids == null)
			{
				snapGridSupport.SnapGrids = new List<SnapGridBase>();
			}

			// do not add if already added
			foreach (var snap_grid in snapGridSupport.SnapGrids)
			{
				if (snap_grid == snapGrid)
				{
					return;
				}
			}

			snapGridSupport.SnapGrids.Add(snapGrid);
		}

		/// <summary>
		/// Replace SnapGrid.
		/// </summary>
		/// <param name="snapGridSupport">SnapGrid support component.</param>
		/// <param name="snapGrid">Snap grid.</param>
		protected virtual void Replace(ISnapGridSupport snapGridSupport, SnapGridBase snapGrid)
		{
			if (snapGridSupport.SnapGrids == null)
			{
				snapGridSupport.SnapGrids = new List<SnapGridBase>();
			}
			else
			{
				snapGridSupport.SnapGrids.Clear();
			}

			snapGridSupport.SnapGrids.Add(snapGrid);
		}

		/// <summary>
		/// Find the SnapGrid.
		/// </summary>
		/// <returns>SnapGrid.</returns>
		/// <param name="eventData">Event data.</param>
		/// <param name="raycasts">Raycast results.</param>
		protected virtual SnapGridBase FindSnapGrid(PointerEventData eventData, List<RaycastResult> raycasts)
		{
			foreach (var raycast in raycasts)
			{
				if (!raycast.isValid)
				{
					continue;
				}

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				var target = raycast.gameObject.GetComponent<SnapGridBase>();
#else
				var target = raycast.gameObject.GetComponent(typeof(SnapGridBase)) as SnapGridBase;
#endif
				if (target != null)
				{
					return target;
				}
			}

			return null;
		}
	}
}