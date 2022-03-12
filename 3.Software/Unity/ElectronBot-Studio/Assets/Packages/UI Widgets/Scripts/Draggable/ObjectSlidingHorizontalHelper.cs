namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Helper for ObjectSliding.
	/// Set allowed positions to current, current + width(ObjectsOnLeft), current - width(ObjectsOnRight)
	/// </summary>
	[RequireComponent(typeof(ObjectSliding))]
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/New UI Widgets/Interactions/Object Sliding Horizontal Helper")]
	[DisallowMultipleComponent]
	public class ObjectSlidingHorizontalHelper : MonoBehaviour
	{
		/// <summary>
		/// Objects on left.
		/// </summary>
		[SerializeField]
		protected List<RectTransform> ObjectsOnLeft = new List<RectTransform>();

		/// <summary>
		/// Objects on right.
		/// </summary>
		[SerializeField]
		protected List<RectTransform> ObjectsOnRight = new List<RectTransform>();

		/// <summary>
		/// Current ObjectSliding.
		/// </summary>
		[HideInInspector]
		protected ObjectSliding Sliding;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		bool isInited;

		/// <summary>
		/// Adds listeners and set positions.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			Sliding = GetComponent<ObjectSliding>();

			Sliding.Direction = ObjectSlidingDirection.Horizontal;

			AddListeners();

			CalculatePositions();
		}

		/// <summary>
		/// Adds listener.
		/// </summary>
		/// <param name="rect">RectTransform</param>
		protected virtual void AddListener(RectTransform rect)
		{
			var rl = Utilities.GetOrAddComponent<ResizeListener>(rect);
			rl.OnResize.AddListener(CalculatePositions);
		}

		/// <summary>
		/// Adds listeners.
		/// </summary>
		protected virtual void AddListeners()
		{
			foreach (var obj in ObjectsOnLeft)
			{
				AddListener(obj);
			}

			foreach (var obj in ObjectsOnRight)
			{
				AddListener(obj);
			}
		}

		/// <summary>
		/// Remove listener.
		/// </summary>
		/// <param name="rect">RectTransform.</param>
		protected virtual void RemoveListener(RectTransform rect)
		{
			var rl = rect.GetComponent<ResizeListener>();
			if (rl != null)
			{
				rl.OnResize.RemoveListener(CalculatePositions);
			}
		}

		/// <summary>
		/// Remove listener.
		/// </summary>
		protected virtual void RemoveListeners()
		{
			foreach (var obj in ObjectsOnLeft)
			{
				RemoveListener(obj);
			}

			foreach (var obj in ObjectsOnRight)
			{
				RemoveListener(obj);
			}
		}

		/// <summary>
		/// Get summary width.
		/// </summary>
		/// <param name="list">Items list.</param>
		/// <returns>Summary width.</returns>
		protected static float SumWidth(List<RectTransform> list)
		{
			var result = 0f;

			for (int i = 0; i < list.Count; i++)
			{
				result += list[i].rect.width;
			}

			return result;
		}

		/// <summary>
		/// Calculate positions.
		/// </summary>
		protected virtual void CalculatePositions()
		{
			var pos = (Sliding.transform as RectTransform).anchoredPosition.x;
			var left = pos + SumWidth(ObjectsOnLeft);
			var right = pos - SumWidth(ObjectsOnRight);

			Sliding.Positions.Clear();
			Sliding.Positions.Add(pos);
			Sliding.Positions.Add(left);
			Sliding.Positions.Add(right);
		}

		/// <summary>
		/// Remove listeners on destroy.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveListeners();
		}
	}
}