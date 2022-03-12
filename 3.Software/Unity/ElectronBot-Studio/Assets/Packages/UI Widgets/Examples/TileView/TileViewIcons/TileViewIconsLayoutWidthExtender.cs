namespace UIWidgets.Examples
{
	using EasyLayoutNS;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TileViewIcons LayoutWidth extender.
	/// </summary>
	[RequireComponent(typeof(EasyLayout))]
	public class TileViewIconsLayoutWidthExtender : MonoBehaviour, ILayoutElement
	{
		/// <summary>
		/// TileViewIcons
		/// </summary>
		[SerializeField]
		protected TileViewIcons TileViewIcons;

		/// <summary>
		/// Flexible height.
		/// </summary>
		public float flexibleHeight
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// Flexible width.
		/// </summary>
		public float flexibleWidth
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// Layout priority.
		/// </summary>
		public int layoutPriority
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// Min height.
		/// </summary>
		public float minHeight
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// Min width.
		/// </summary>
		public float minWidth
		{
			get
			{
				if (TileViewIcons == null)
				{
					return -1;
				}

				var width_min = WidthMin();
				var width_step = WidthStep();

				var k = Mathf.Ceil((Layout.minWidth - width_min) / width_step);
				return width_min + (width_step * k);
			}
		}

		/// <summary>
		/// Preferred height.
		/// </summary>
		public float preferredHeight
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// Preferred width.
		/// </summary>
		public float preferredWidth
		{
			get
			{
				if (TileViewIcons == null)
				{
					return -1;
				}

				var width_min = WidthMin();
				var width_step = WidthStep();

				var k = Mathf.Ceil((Layout.preferredWidth - width_min) / width_step);
				return width_min + (width_step * k);
			}
		}

		EasyLayout layout;

		/// <summary>
		/// Layout.
		/// </summary>
		public EasyLayout Layout
		{
			get
			{
				if (layout == null)
				{
					layout = GetComponent<EasyLayout>();
				}

				return layout;
			}
		}

		float WidthMin()
		{
			var scroll_width = (TileViewIcons.ScrollRect.transform as RectTransform).rect.width;
			var item_width = (TileViewIcons.DefaultItem.transform as RectTransform).rect.width;
			var n = (scroll_width - item_width) / (item_width + Layout.Spacing.x);
			n = Mathf.Max(0, Mathf.Floor(n)) + 1;
			return (item_width * n) + (Layout.Spacing.x * (n - 1));
		}

		float WidthStep()
		{
			return WidthMin() + Layout.Spacing.x;
		}

		/// <summary>
		/// CalculateLayoutInputHorizontal.
		/// </summary>
		public void CalculateLayoutInputHorizontal()
		{
		}

		/// <summary>
		/// CalculateLayoutInputVertical.
		/// </summary>
		public void CalculateLayoutInputVertical()
		{
		}
	}
}