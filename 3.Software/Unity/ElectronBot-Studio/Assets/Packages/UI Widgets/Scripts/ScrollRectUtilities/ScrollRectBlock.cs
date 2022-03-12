namespace UIWidgets
{
	using System;
	using EasyLayoutNS;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// ScrollRectBlock.
	/// </summary>
	public abstract class ScrollRectBlock : MonoBehaviourConditional
	{
		/// <summary>
		/// ScrollRect.
		/// </summary>
		[SerializeField]
		protected ScrollRect ScrollRect;

		/// <summary>
		/// Header.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("Header")]
		protected RectTransform Block;

		/// <summary>
		/// Is ScrollRect has horizontal direction.
		/// </summary>
		[SerializeField]
		public bool IsHorizontal = false;

		/// <summary>
		/// Display type.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("HeaderType")]
		protected ScrollRectHeaderType DisplayType = ScrollRectHeaderType.Reveal;

		/// <summary>
		/// Min size of the header.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("HeaderMinSize")]
		[EditorConditionEnum("DisplayType", new int[] { (int)ScrollRectHeaderType.Resize })]
		public float MinSize = 30f;

		/// <summary>
		/// Last position of the ScrollRect.content.
		/// </summary>
		[NonSerialized]
		protected Vector2 LastPosition;

		/// <summary>
		/// Header size.
		/// </summary>
		[NonSerialized]
		protected Vector2 MaxSize;

		/// <summary>
		/// Layout.
		/// </summary>
		[NonSerialized]
		protected EasyLayout Layout;

		/// <summary>
		/// Layout margin delta.
		/// </summary>
		[NonSerialized]
		protected float MarginDelta;

		/// <summary>
		/// ScrollRect transform.
		/// </summary>
		[NonSerialized]
		protected RectTransform ScrollRectTransform;

		[NonSerialized]
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

			if ((ScrollRect != null) && (Block != null))
			{
				ScrollRect.onValueChanged.AddListener(Scroll);
				ScrollRectTransform = ScrollRect.transform as RectTransform;

				LastPosition = GetPosition();
				MaxSize = Block.rect.size;
				Layout = ScrollRect.content.GetComponent<EasyLayout>();

				InitReveal();

				InitLayout();

				Scroll(Vector2.zero);
			}
		}

		/// <summary>
		/// Init for reveal display type.
		/// </summary>
		protected abstract void InitReveal();

		/// <summary>
		/// Init layout.
		/// </summary>
		protected abstract void InitLayout();

		/// <summary>
		/// Update layout.
		/// </summary>
		/// <param name="size">Size.</param>
		protected abstract void UpdateLayout(float size);

		/// <summary>
		/// Get position.
		/// </summary>
		/// <returns>position.</returns>
		protected virtual Vector2 GetPosition()
		{
			return ScrollRect.content.anchoredPosition;
		}

		/// <summary>
		/// Process ScrollRect.onValueChanged event.
		/// </summary>
		/// <param name="scrollPosition">ScrollRect value.</param>
		protected void Scroll(Vector2 scrollPosition)
		{
			var position = GetPosition();
			switch (DisplayType)
			{
				case ScrollRectHeaderType.Resize:
					Resize(position);
					break;
				case ScrollRectHeaderType.Reveal:
					Reveal(position);
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown ScrollRectHeaderType: {0}", EnumHelper<ScrollRectHeaderType>.ToString(DisplayType)));
			}

			LastPosition = position;
		}

		/// <summary>
		/// Get visible rate.
		/// </summary>
		/// <param name="scrollPosition">Scroll position.</param>
		/// <returns>Visible rate.</returns>
		protected float VisibleRate(Vector2 scrollPosition)
		{
			var scroll = IsHorizontal ? -scrollPosition.x : scrollPosition.y;
			var size = IsHorizontal ? MaxSize.x : MaxSize.y;
			var visible_rate = 1f - Mathf.Clamp01(scroll / size);

			return visible_rate;
		}

		/// <summary>
		/// Get reveal position.
		/// </summary>
		/// <param name="rate">Visible rate.</param>
		/// <returns>Position.</returns>
		protected abstract float RevealPosition(float rate);

		/// <summary>
		/// Reveal header.
		/// </summary>
		/// <param name="scrollPosition">Current scroll position.</param>
		protected void Reveal(Vector2 scrollPosition)
		{
			var current_pos = Block.anchoredPosition;
			var visible_rate = VisibleRate(scrollPosition);

			if (IsHorizontal)
			{
				current_pos.x = RevealPosition(visible_rate);
			}
			else
			{
				current_pos.y = RevealPosition(visible_rate);
			}

			Block.anchoredPosition = current_pos;

			var new_size = (IsHorizontal ? MaxSize.x : MaxSize.y) * visible_rate;
			UpdateLayout(new_size);
		}

		/// <summary>
		/// Resize header.
		/// </summary>
		/// <param name="scrollPosition">Current ScrollRect.content position.</param>
		protected void Resize(Vector2 scrollPosition)
		{
			var visible_rate = VisibleRate(scrollPosition);

			var new_size = Mathf.Lerp(MinSize, IsHorizontal ? MaxSize.x : MaxSize.y, visible_rate);

			var axis = IsHorizontal ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
			Block.SetSizeWithCurrentAnchors(axis, new_size);

			UpdateLayout(new_size);
		}

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (ScrollRect != null)
			{
				ScrollRect.onValueChanged.RemoveListener(Scroll);
			}
		}
	}
}