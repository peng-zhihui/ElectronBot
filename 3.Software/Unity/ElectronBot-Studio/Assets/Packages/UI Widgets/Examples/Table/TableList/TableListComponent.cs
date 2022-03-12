namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TableList component.
	/// </summary>
	public class TableListComponent : ListViewItem, IViewData<List<int>>
	{
		/// <summary>
		/// The text components.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with TextAdapterComponents.")]
		public List<Text> TextComponents = new List<Text>();

		/// <summary>
		/// The text components.
		/// </summary>
		[SerializeField]
		public List<TextAdapter> TextAdapterComponents = new List<TextAdapter>();

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				Foreground = new Graphic[TextAdapterComponents.Count];

				for (int i = 0; i < TextAdapterComponents.Count; i++)
				{
					Foreground[i] = UtilitiesUI.GetGraphic(TextAdapterComponents[i]);
				}

				GraphicsForegroundVersion = 1;
			}
		}

		/// <summary>
		/// Init graphics background.
		/// </summary>
		protected override void GraphicsBackgroundInit()
		{
			if (GraphicsBackgroundVersion == 0)
			{
				graphicsBackground = Compatibility.EmptyArray<Graphic>();
				GraphicsBackgroundVersion = 1;
			}
		}

		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		public GameObject[] ObjectsToResize
		{
			get
			{
				var result = new GameObject[TextAdapterComponents.Count];

				for (int i = 0; i < TextAdapterComponents.Count; i++)
				{
					result[i] = TextAdapterComponents[i].transform.parent.gameObject;
				}

				return result;
			}
		}

		/// <summary>
		/// The item.
		/// </summary>
		[SerializeField]
		protected List<int> Item;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(List<int> item)
		{
			Item = item;
			UpdateView();
		}

		/// <summary>
		/// Update text components text.
		/// </summary>
		public void UpdateView()
		{
			for (int i = 0; i < TextAdapterComponents.Count; i++)
			{
				SetData(TextAdapterComponents[i], i);
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="index">Index.</param>
		protected void SetData(TextAdapter text, int index)
		{
			text.text = Item.Count > index ? Item[index].ToString() : "none";
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			if (TextAdapterComponents.Count == 0)
			{
				foreach (var t in TextComponents)
				{
					if (t != null)
					{
						TextAdapterComponents.Add(Utilities.GetOrAddComponent<TextAdapter>(t));
					}
				}
			}
#pragma warning restore 0612, 0618
		}
	}
}