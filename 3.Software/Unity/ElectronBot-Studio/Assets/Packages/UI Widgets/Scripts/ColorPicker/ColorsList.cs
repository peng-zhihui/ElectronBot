namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Colors list.
	/// </summary>
	public class ColorsList : MonoBehaviour, IStylable
	{
		/// <summary>
		/// ColorPicker.
		/// </summary>
		[SerializeField]
		protected ColorPicker colorPicker;

		/// <summary>
		/// ColorPicker.
		/// </summary>
		public ColorPicker ColorPicker
		{
			get
			{
				return colorPicker;
			}

			set
			{
				colorPicker = value;
			}
		}

		/// <summary>
		/// ColorPickerRange.
		/// </summary>
		[SerializeField]
		protected ColorPickerRange colorPickerRange;

		/// <summary>
		/// ColorPickerRange.
		/// </summary>
		public ColorPickerRange ColorPickerRange
		{
			get
			{
				return colorPickerRange;
			}

			set
			{
				colorPickerRange = value;
			}
		}

		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		protected ListViewColors listView;

		/// <summary>
		/// ListView.
		/// </summary>
		public ListViewColors ListView
		{
			get
			{
				return listView;
			}

			set
			{
				if (listView != null)
				{
					listView.OnSelectObject.RemoveListener(SetColor);
				}

				listView = value;

				if (listView != null)
				{
					listView.OnSelectObject.AddListener(SetColor);
				}
			}
		}

		/// <summary>
		/// Button to add color to ListView.
		/// </summary>
		[SerializeField]
		protected Button addButton;

		/// <summary>
		/// Button to add color to ListView.
		/// </summary>
		public Button AddButton
		{
			get
			{
				return addButton;
			}

			set
			{
				if (addButton != null)
				{
					addButton.onClick.RemoveListener(AddColor);
				}

				addButton = value;

				if (addButton != null)
				{
					addButton.onClick.AddListener(AddColor);
				}
			}
		}

		/// <summary>
		/// Starts this instance.
		/// </summary>
		public virtual void Start()
		{
			ColorPicker = colorPicker;
			ColorPickerRange = colorPickerRange;
			ListView = listView;
			AddButton = addButton;
		}

		/// <summary>
		/// Add color to ListView.
		/// </summary>
		protected virtual void AddColor()
		{
			if (ColorPicker != null)
			{
				ListView.DataSource.Add(ColorPicker.Color);
			}
			else if (ColorPickerRange != null)
			{
				ListView.DataSource.Add(ColorPickerRange.Color);
			}
		}

		/// <summary>
		/// Set color to ColorPicker.
		/// </summary>
		/// <param name="index">Item index.</param>
		protected virtual void SetColor(int index)
		{
			if (ColorPicker != null)
			{
				ColorPicker.Color = ListView.DataSource[index];
			}
			else if (ColorPickerRange != null)
			{
				ColorPickerRange.Color = ListView.DataSource[index];
			}
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (listView != null)
			{
				listView.SetStyle(style);
			}

			style.ButtonSmall.ApplyTo(addButton);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (listView != null)
			{
				listView.GetStyle(style);
			}

			style.ButtonSmall.GetFrom(addButton);

			return true;
		}

		#endregion
	}
}