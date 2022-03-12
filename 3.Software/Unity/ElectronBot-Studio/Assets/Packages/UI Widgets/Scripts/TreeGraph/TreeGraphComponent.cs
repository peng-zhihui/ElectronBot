namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TreeGraph component.
	/// </summary>
	/// <typeparam name="T">Node type.</typeparam>
	[RequireComponent(typeof(MultipleConnector))]
	public abstract class TreeGraphComponent<T> : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		[SerializeField]
		protected Graphic[] graphicsForeground = Compatibility.EmptyArray<Graphic>();

		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public virtual Graphic[] GraphicsForeground
		{
			get
			{
				GraphicsForegroundInit();

				return graphicsForeground;
			}
		}

		/// <summary>
		/// Background graphics for coloring.
		/// </summary>
		[SerializeField]
		protected Graphic[] graphicsBackground = Compatibility.EmptyArray<Graphic>();

		/// <summary>
		/// Get background graphics for coloring.
		/// </summary>
		public virtual Graphic[] GraphicsBackground
		{
			get
			{
				GraphicsBackgroundInit();

				return graphicsBackground;
			}
		}

		/// <summary>
		/// Background.
		/// </summary>
		[SerializeField]
		public Image Background;

		RectTransform rectTransform;

		/// <summary>
		/// Gets the RectTransform.
		/// </summary>
		/// <value>The RectTransform.</value>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// Node.
		/// </summary>
		protected TreeNode<T> Node;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (Background != null)
			{
				return;
			}

			var bg = transform.Find("Background");
			if (bg == null)
			{
				return;
			}

			Background = bg.GetComponent<Image>();
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public virtual void LocaleChanged()
		{
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="node">Node.</param>
		public abstract void SetData(TreeNode<T> node);

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public virtual void MovedToCache()
		{
		}

		/// <summary>
		/// Toggle node visibility.
		/// </summary>
		public virtual void ToggleVisibility()
		{
			Node.IsVisible = !Node.IsVisible;
		}

		/// <summary>
		/// Toggle expanded.
		/// </summary>
		public virtual void ToggleExpanded()
		{
			Node.IsExpanded = !Node.IsExpanded;
		}

		/// <summary>
		/// Graphics background version.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected byte GraphicsBackgroundVersion = 0;

		/// <summary>
		/// Init graphics background.
		/// </summary>
		protected virtual void GraphicsBackgroundInit()
		{
			if (GraphicsBackgroundVersion == 0)
			{
				graphicsBackground = new Graphic[] { Background };
				GraphicsBackgroundVersion = 1;
			}
		}

		/// <summary>
		/// Graphics foreground version.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected byte GraphicsForegroundVersion = 0;

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected virtual void GraphicsForegroundInit()
		{
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			GraphicsBackgroundInit();
			GraphicsForegroundInit();

#if UNITY_2018_3_OR_NEWER
			if (UnityEditor.PrefabUtility.IsPartOfAnyPrefab(this))
#endif
			{
				UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
			}

			if (Compatibility.IsPrefab(this))
			{
				UnityEditor.EditorUtility.SetDirty(this);
			}
		}
#endif

		#region IStylable implementation

		/// <summary>
		/// Set the style.
		/// </summary>
		/// <param name="styleBackground">Style for the background.</param>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public virtual void SetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			styleBackground.ApplyTo(Background);

			foreach (var gf in GraphicsForeground)
			{
				if (gf != null)
				{
					styleText.ApplyTo(gf.gameObject);
				}
			}

			var connector = GetComponent<MultipleConnector>();
			if (connector != null)
			{
				connector.SetStyle(style);
			}
		}

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public virtual bool SetStyle(Style style)
		{
			SetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);

			return true;
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleBackground">Style for the background.</param>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public virtual void GetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			styleBackground.GetFrom(Background);

			foreach (var gf in GraphicsForeground)
			{
				if (gf != null)
				{
					styleText.GetFrom(gf.gameObject);
				}
			}

			var connector = GetComponent<MultipleConnector>();
			if (connector != null)
			{
				connector.GetStyle(style);
			}
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="style">Style data.</param>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		public virtual bool GetStyle(Style style)
		{
			GetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);

			return true;
		}
		#endregion
	}
}