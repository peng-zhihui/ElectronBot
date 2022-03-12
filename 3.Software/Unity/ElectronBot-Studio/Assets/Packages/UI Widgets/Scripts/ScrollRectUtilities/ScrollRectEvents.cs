namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// ScrollRect events.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Helpers/ScrollRect Events")]
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollRectEvents : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		/// <summary>
		/// Pull thresholds.
		/// </summary>
		[Serializable]
		public struct PullThreshold : IEquatable<PullThreshold>
		{
			/// <summary>
			/// Up.
			/// </summary>
			public float Up;

			/// <summary>
			/// Down.
			/// </summary>
			public float Down;

			/// <summary>
			/// Left.
			/// </summary>
			public float Left;

			/// <summary>
			/// Right.
			/// </summary>
			public float Right;

			/// <summary>
			/// Initializes a new instance of the <see cref="PullThreshold"/> struct.
			/// </summary>
			/// <param name="up">Up.</param>
			/// <param name="down">Down.</param>
			/// <param name="left">Left.</param>
			/// <param name="right">Right.</param>
			public PullThreshold(float up = 50f, float down = 50f, float left = 50f, float right = 50f)
			{
				Up = up;
				Down = down;
				Left = left;
				Right = right;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj)
			{
				if (obj is PullThreshold)
				{
					return Equals((PullThreshold)obj);
				}

				return false;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(PullThreshold other)
			{
				return Up == other.Up && Down == other.Down && Left == other.Left && Right == other.Right;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode()
			{
				return Up.GetHashCode() ^ Down.GetHashCode() ^ Left.GetHashCode() ^ Right.GetHashCode();
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(PullThreshold left, PullThreshold right)
			{
				return left.Equals(right);
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are now equal; otherwise, false.</returns>
			public static bool operator !=(PullThreshold left, PullThreshold right)
			{
				return !left.Equals(right);
			}
		}

		/// <summary>
		/// Pull directions.
		/// </summary>
		public enum PullDirection
		{
			/// <summary>
			/// None.
			/// </summary>
			None = 0,

			/// <summary>
			/// Up.
			/// </summary>
			Up = 1,

			/// <summary>
			/// Down.
			/// </summary>
			Down = 2,

			/// <summary>
			/// Left.
			/// </summary>
			Left = 3,

			/// <summary>
			/// Right.
			/// </summary>
			Right = 4,
		}

		/// <summary>
		/// Pull event.
		/// </summary>
		[Serializable]
		public class PullEvent : UnityEvent<PullDirection>
		{
		}

		/// <summary>
		/// Pull event.
		/// </summary>
		[Serializable]
		public class PullingEvent : UnityEvent<ScrollRectEvents, PullDirection>
		{
		}

		/// <summary>
		/// The required pull distance to raise events.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Thresholds.")]
		[HideInInspector]
		public float RequiredMovement = 50f;

		/// <summary>
		/// Thresholds.
		/// </summary>
		[SerializeField]
		public PullThreshold Thresholds;

		/// <summary>
		/// OnPulling event.
		/// </summary>
		[SerializeField]
		public PullingEvent OnPulling = new PullingEvent();

		/// <summary>
		/// OnPull event.
		/// </summary>
		[SerializeField]
		public PullEvent OnPull = new PullEvent();

		/// <summary>
		/// Event raised when drag distance is equal or more than RequiredMovement.
		/// </summary>
		[SerializeField]
		public PullEvent OnPullAllowed = new PullEvent();

		/// <summary>
		/// Event raised when drag distance is less than RequiredMovement after OnPullValid was called.
		/// </summary>
		[SerializeField]
		public PullEvent OnPullCancel = new PullEvent();

		/// <summary>
		/// OnPullUp event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnPullUp = new UnityEvent();

		/// <summary>
		/// OnPullDown event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnPullDown = new UnityEvent();

		/// <summary>
		/// OnPullLeft event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnPullLeft = new UnityEvent();

		/// <summary>
		/// OnPullRight event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnPullRight = new UnityEvent();

		ScrollRect scrollRect;

		/// <summary>
		/// Gets the ScrollRect.
		/// </summary>
		/// <value>ScrollRect.</value>
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

		PullDirection pullDirection = PullDirection.None;

		bool isPullValid;

		/// <summary>
		/// Called by a BaseInputModule before a drag is started.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			ResetDrag();
		}

		/// <summary>
		/// Called by a BaseInputModule when a drag is ended.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (isPullValid)
			{
				OnPull.Invoke(pullDirection);

				switch (pullDirection)
				{
					case PullDirection.Up:
						OnPullUp.Invoke();
						break;
					case PullDirection.Down:
						OnPullDown.Invoke();
						break;
					case PullDirection.Left:
						OnPullLeft.Invoke();
						break;
					case PullDirection.Right:
						OnPullRight.Invoke();
						break;
					case PullDirection.None:
						break;
					default:
						Debug.LogWarning(string.Format("Unsupported pull direction: {0}", EnumHelper<PullDirection>.ToString(pullDirection)));
						break;
				}
			}

			ResetDrag();
		}

		/// <summary>
		/// Resets the drag values.
		/// </summary>
		protected virtual void ResetDrag()
		{
			pullDirection = PullDirection.None;
			isPullValid = false;
		}

		/// <summary>
		/// Pull distance.
		/// </summary>
		public float PullDistance
		{
			get;
			protected set;
		}

		/// <summary>
		/// When dragging is occurring this will be called every time the cursor is moved.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			var size = (ScrollRect.transform as RectTransform).rect.size;

			var max_x = Mathf.Max(0f, ScrollRect.content.rect.width - size.x);
			var max_y = Mathf.Max(0f, ScrollRect.content.rect.height - size.y);

			var valid_distance = false;
			if (ScrollRect.content.anchoredPosition.y < 0f)
			{
				PullDistance = -ScrollRect.content.anchoredPosition.y;
				valid_distance = PullDistance >= Thresholds.Up;
				pullDirection = PullDirection.Up;
			}
			else if (ScrollRect.content.anchoredPosition.y > max_y)
			{
				PullDistance = ScrollRect.content.anchoredPosition.y - max_y;
				valid_distance = PullDistance >= Thresholds.Down;
				pullDirection = PullDirection.Down;
			}
			else if (ScrollRect.content.anchoredPosition.x < 0f)
			{
				PullDistance = -ScrollRect.content.anchoredPosition.x;
				valid_distance = PullDistance >= Thresholds.Left;
				pullDirection = PullDirection.Left;
			}
			else if (ScrollRect.content.anchoredPosition.x > max_x)
			{
				PullDistance = ScrollRect.content.anchoredPosition.x - max_x;
				valid_distance = PullDistance >= Thresholds.Right;
				pullDirection = PullDirection.Right;
			}
			else
			{
				PullDistance = 0f;
			}

			OnPulling.Invoke(this, pullDirection);

			if (valid_distance)
			{
				if (!isPullValid)
				{
					OnPullAllowed.Invoke(pullDirection);
					isPullValid = true;
				}
			}
			else
			{
				if (isPullValid)
				{
					OnPullCancel.Invoke(pullDirection);
					isPullValid = false;
					pullDirection = PullDirection.None;
				}
			}
		}

		[SerializeField]
		[HideInInspector]
		int version = 0;

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Upgrade();
		}
#endif

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		protected virtual void Upgrade()
		{
			if (version == 0)
			{
#pragma warning disable 0618
				Thresholds = new PullThreshold(RequiredMovement, RequiredMovement, RequiredMovement, RequiredMovement);
#pragma warning restore 0618
				version = 1;
			}
		}
	}
}