namespace UIWidgets
{
	using System.IO;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for FileListViewPathComponent.
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class FileListViewPathComponentBase : ComponentPool<FileListViewPathComponentBase>, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter NameAdapter;

		/// <summary>
		/// Path to displayed directory.
		/// </summary>
		public string FullName
		{
			get;
			protected set;
		}

		/// <summary>
		/// Parent FileListViewPath.
		/// </summary>
		[HideInInspector]
		public FileListViewPath Owner;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			var layout = GetComponent<EasyLayoutNS.EasyLayout>();
			if (layout != null)
			{
				layout.ChildrenWidth = EasyLayoutNS.ChildrenSize.SetPreferred;
			}

			var mask = Utilities.GetOrAddComponent<Mask>(this);
			mask.showMaskGraphic = true;
		}

		/// <summary>
		/// Set path.
		/// </summary>
		/// <param name="path">Path.</param>
		public virtual void SetPath(string path)
		{
			FullName = path;
			var dir = Path.GetFileName(path);
			NameAdapter.text = !string.IsNullOrEmpty(dir) ? dir : path;
		}

		/// <summary>
		/// OnPointerDown event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
		}

		/// <summary>
		/// OnPointerUp event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
		}

		/// <summary>
		/// OnPointerClick event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				Owner.Open(FullName);
			}
		}

		/// <summary>
		/// Set the style.
		/// </summary>
		/// <param name="styleBackground">Style for the background.</param>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public virtual void SetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			foreach (var c in Cache)
			{
				c.SetStyle(styleBackground, styleText, style);
			}

			styleBackground.ApplyTo(GetComponent<Image>());

			if (NameAdapter != null)
			{
				styleText.ApplyTo(NameAdapter.gameObject);
			}
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleBackground">Style for the background.</param>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public virtual void GetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			styleBackground.GetFrom(GetComponent<Image>());

			if (NameAdapter != null)
			{
				styleText.GetFrom(NameAdapter.gameObject);
			}
		}
	}
}