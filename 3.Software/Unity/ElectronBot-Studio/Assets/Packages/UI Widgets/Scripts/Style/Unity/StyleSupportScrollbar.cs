namespace UIWidgets.Styles
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the scrollbar.
	/// </summary>
	[RequireComponent(typeof(Scrollbar))]
	public class StyleSupportScrollbar : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Style for the main background.
		/// </summary>
		[SerializeField]
		public Image MainBackground;

		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public Image Background;

		/// <summary>
		/// Style for the handle.
		/// </summary>
		[SerializeField]
		public Image Handle;

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			var scrollbar = GetComponent<Scrollbar>();
			if (scrollbar == null)
			{
				return false;
			}

			var scrollbar_style = UtilitiesUI.IsHorizontal(scrollbar)
				? style.HorizontalScrollbar
				: style.VerticalScrollbar;

			SetStyle(scrollbar_style);

			return true;
		}

		/// <summary>
		/// Set the style.
		/// </summary>
		/// <param name="style">Style.</param>
		public virtual void SetStyle(StyleScrollbar style)
		{
			style.MainBackground.ApplyTo(MainBackground);
			style.Background.ApplyTo(Background);
			style.Handle.ApplyTo(Handle);
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			var scrollbar = GetComponent<Scrollbar>();
			if (scrollbar == null)
			{
				return false;
			}

			var scrollbar_style = UtilitiesUI.IsHorizontal(scrollbar)
				? style.HorizontalScrollbar
				: style.VerticalScrollbar;

			GetStyle(scrollbar_style);

			return true;
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="style">Style.</param>
		public virtual void GetStyle(StyleScrollbar style)
		{
			style.MainBackground.GetFrom(MainBackground);
			style.Background.GetFrom(Background);
			style.Handle.GetFrom(Handle);
		}
	}
}