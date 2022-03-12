namespace UIWidgets.Examples
{
	using System.Collections;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Minimal size for the Scrollbar.
	/// </summary>
	[RequireComponent(typeof(ScrollRect))]
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/New UI Widgets/Helpers/Scrollbar Minimal size")]
	public class ScrollbarMinSize : UIWidgetsMonoBehaviour, IValidateable
	{
		[Range(0f, 1f)]
		[SerializeField]
		float horizontalMinSize = 0.2f;

		/// <summary>
		/// Horizontal minimal size.
		/// </summary>
		public float HorizontalMinSize
		{
			get
			{
				return horizontalMinSize;
			}

			set
			{
				horizontalMinSize = value;
				SetScrollbarSizes();
			}
		}

		[Range(0f, 1f)]
		[SerializeField]
		float verticalMinSize = 0.2f;

		/// <summary>
		/// Vertical minimal size.
		/// </summary>
		public float VerticalMinSize
		{
			get
			{
				return verticalMinSize;
			}

			set
			{
				verticalMinSize = value;
				SetScrollbarSizes();
			}
		}

		/// <summary>
		/// ScrollRect.
		/// </summary>
		[FormerlySerializedAs("scrollRect")]
		protected ScrollRect ScrollRect;

		bool isInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
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

			isInited = true;

			ScrollRect = GetComponent<ScrollRect>();
			ScrollRect.onValueChanged.AddListener(SetScrollbarSize);
			StartCoroutine(SetScrollbarSizeAtNextFrame());
		}

		/// <summary>
		/// Set scrollbar size at next frame.
		/// </summary>
		/// <returns>IEnumerator.</returns>
		protected IEnumerator SetScrollbarSizeAtNextFrame()
		{
			yield return null;
			SetScrollbarSizes();

			// additional delay required for Unity 2018.3.11f1
			yield return null;
			SetScrollbarSizes();
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (ScrollRect != null)
			{
				ScrollRect.onValueChanged.RemoveListener(SetScrollbarSize);
			}
		}

		/// <summary>
		/// Set scrollbar size.
		/// </summary>
		/// <param name="unused">Scroll value. Unused.</param>
		protected virtual void SetScrollbarSize(Vector2 unused)
		{
			SetScrollbarSizes();
		}

		/// <summary>
		/// Set scrollbar size.
		/// </summary>
		public virtual void SetScrollbarSizes()
		{
			if (!isInited)
			{
				Init();
			}

			if (ScrollRect == null)
			{
				return;
			}

			if (ScrollRect.horizontalScrollbar != null)
			{
				ScrollRect.horizontalScrollbar.size = Mathf.Max(ScrollRect.horizontalScrollbar.size, horizontalMinSize);
			}

			if (ScrollRect.verticalScrollbar != null)
			{
				ScrollRect.verticalScrollbar.size = Mathf.Max(ScrollRect.verticalScrollbar.size, verticalMinSize);
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		public virtual void Validate()
		{
			if (gameObject.activeInHierarchy)
			{
				ScrollRect = GetComponent<ScrollRect>();
				SetScrollbarSizes();
			}
		}

		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			horizontalMinSize = Mathf.Clamp01(horizontalMinSize);
			verticalMinSize = Mathf.Clamp01(verticalMinSize);
		}
#endif
	}
}