namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Auto-resizes ListView or TileView according to item counts until specified maximum size reached.
	/// </summary>
	[RequireComponent(typeof(ListViewBase))]
	public class ListViewAutoResize : MonoBehaviour, ILayoutElement
	{
		/// <summary>
		/// Maximum size.
		/// </summary>
		[SerializeField]
		public float MaxSize = 250f;

		/// <summary>
		/// RectTransform.
		/// </summary>
		protected RectTransform RectTransform;

		/// <summary>
		/// Difference in size between ListView and ListView.Container parent.
		/// </summary>
		protected Vector2 SizeDifference;

		/// <summary>
		/// EasyLayout start margin.
		/// </summary>
		protected Vector2 LayoutMarginStart;

		/// <summary>
		/// EasyLayout end margin.
		/// </summary>
		protected Vector2 LayoutMarginEnd;

		/// <summary>
		/// Current size.
		/// </summary>
		protected Vector2 CurrentSize = new Vector2(-1f, -1f);

		/// <summary>
		/// Properties tracker.
		/// </summary>
		protected DrivenRectTransformTracker PropertiesTracker;

		[SerializeField]
		[FormerlySerializedAs("UpdateRectTransform")]
		bool updateRectTransform = true;

		/// <summary>
		/// Update RectTransform.
		/// </summary>
		public bool UpdateRectTransform
		{
			get
			{
				return updateRectTransform;
			}

			set
			{
				if (updateRectTransform != value)
				{
					updateRectTransform = value;

					UpdatePropertiesTracker();
				}
			}
		}

		ListViewBase listView;

		/// <summary>
		/// ListView.
		/// </summary>
		protected ListViewBase ListView
		{
			get
			{
				if (listView == null)
				{
					listView = GetComponent<ListViewBase>();
				}

				return listView;
			}
		}

		/// <summary>
		/// Minimal width.
		/// </summary>
		public float minWidth
		{
			get
			{
				return CurrentSize.x;
			}
		}

		/// <summary>
		/// Preferred width.
		/// </summary>
		public float preferredWidth
		{
			get
			{
				return CurrentSize.x;
			}
		}

		/// <summary>
		/// Flexible width.
		/// </summary>
		public float flexibleWidth
		{
			get
			{
				return 0f;
			}
		}

		/// <summary>
		/// Minimal height.
		/// </summary>
		public float minHeight
		{
			get
			{
				return CurrentSize.y;
			}
		}

		/// <summary>
		/// Preferred height.
		/// </summary>
		public float preferredHeight
		{
			get
			{
				return CurrentSize.y;
			}
		}

		/// <summary>
		/// Flexible height.
		/// </summary>
		public float flexibleHeight
		{
			get
			{
				return 0f;
			}
		}

		/// <summary>
		/// Layout priority.
		/// </summary>
		[SerializeField]
		protected int LayoutPriority = 2;

		/// <summary>
		/// Layout priority.
		/// </summary>
		public int layoutPriority
		{
			get
			{
				return LayoutPriority;
			}

			set
			{
				LayoutPriority = value;
			}
		}

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

			UpdateSizeDifference();

			ListView.OnUpdateView.AddListener(Resize);
			Resize();
		}

		/// <summary>
		/// Update properties tracker.
		/// </summary>
		protected virtual void UpdatePropertiesTracker()
		{
			PropertiesTracker.Clear();

			if (UpdateRectTransform)
			{
				var property_type = ListView.IsHorizontal()
					? DrivenTransformProperties.SizeDeltaX
					: DrivenTransformProperties.SizeDeltaY;
				PropertiesTracker.Add(this, RectTransform, property_type);
			}
		}

		/// <summary>
		/// Update size difference.
		/// </summary>
		public virtual void UpdateSizeDifference()
		{
			var scroll_rect = ListView.GetScrollRect();
			var layout = scroll_rect.content.GetComponent<EasyLayoutNS.EasyLayout>();
			LayoutMarginStart = new Vector2(layout.MarginLeft, layout.MarginTop);
			LayoutMarginEnd = new Vector2(layout.MarginRight, layout.MarginBottom);
			RectTransform = transform as RectTransform;

			var content_parent = scroll_rect.content.parent as RectTransform;
			SizeDifference = RectTransform.rect.size - content_parent.rect.size;
		}

		/// <summary>
		/// Calculate size.
		/// </summary>
		/// <returns>Size.</returns>
		protected virtual float UpdateSize()
		{
			if (!Application.isPlaying)
			{
				return 0f;
			}

			var margin_start = ListView.IsHorizontal()
				? LayoutMarginStart.x
				: LayoutMarginStart.y;
			var diff = ListView.IsHorizontal()
				? LayoutMarginEnd.x + SizeDifference.x
				: LayoutMarginEnd.y + SizeDifference.y;

			var count = ListView.GetItemsCount();
			var size = count == 0
				? margin_start
				: ListView.GetItemPositionBorderEnd(count - 1);
			size += diff;

			size = Mathf.Min(size, MaxSize);

			CurrentSize = ListView.IsHorizontal()
				? new Vector2(size, -1f)
				: new Vector2(-1f, size);

			return size;
		}

		/// <summary>
		/// Resize.
		/// </summary>
		protected virtual void Resize()
		{
			UpdatePropertiesTracker();

			var size = UpdateSize();

			if (UpdateRectTransform)
			{
				var axis = ListView.IsHorizontal() ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
				RectTransform.SetSizeWithCurrentAnchors(axis, size);
			}

			LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
		}

		/// <summary>
		/// Calculate size on horizontal axis.
		/// </summary>
		public void CalculateLayoutInputHorizontal()
		{
			UpdateSize();
		}

		/// <summary>
		/// Calculate size on vertical axis.
		/// </summary>
		public void CalculateLayoutInputVertical()
		{
			UpdateSize();
		}
	}
}