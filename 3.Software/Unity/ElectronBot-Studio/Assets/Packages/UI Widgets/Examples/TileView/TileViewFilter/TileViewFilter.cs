namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TileView filter.
	/// </summary>
	[RequireComponent(typeof(InputFieldAdapter))]
	public class TileViewFilter : MonoBehaviour
	{
		/// <summary>
		/// TileView.
		/// </summary>
		[SerializeField]
		protected ListViewString TileView;

		ObservableList<string> dataSource = new ObservableList<string>();

		/// <summary>
		/// DataSource.
		/// </summary>
		public ObservableList<string> DataSource
		{
			get
			{
				return dataSource;
			}
		}

		InputFieldAdapter inputField;

		/// <summary>
		/// Input field.
		/// </summary>
		public InputFieldAdapter InputField
		{
			get
			{
				if (inputField == null)
				{
					inputField = GetComponent<InputFieldAdapter>();
				}

				return inputField;
			}
		}

		bool isInited;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			// get items from TileView.DataSource if items list is empty
			if (DataSource.Count == 0)
			{
				// copy items from TileView
				// DataSource.AddRange(TileView.DataSource);

				// or get items from file
				var loader = TileView.GetComponent<ListViewStringDataFile>();
				DataSource.AddRange(loader.GetItemsFromFile(loader.File));
			}

			// add callback to update TileView on input change
			InputField.onValueChanged.AddListener(Filter);

			// add callback to update TileView on DataSource change
			DataSource.OnChange += DataSourceChanged;

			DataSourceChanged();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void OnDestroy()
		{
			InputField.onValueChanged.RemoveListener(Filter);
		}

		void DataSourceChanged()
		{
			Filter(InputField.text);
		}

		/// <summary>
		/// Check if item match input.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="input">Input.</param>
		/// <returns>true if item match input; otherwise false.</returns>
		bool IsMatch(string item, string input)
		{
			return UtilitiesCompare.Contains(item, input, false);
		}

		/// <summary>
		/// Filter items by input.
		/// </summary>
		/// <param name="input">Input.</param>
		public void Filter(string input)
		{
			TileView.DataSource.BeginUpdate();

			// remove items after previous filter
			TileView.DataSource.Clear();

			// if input is empty add all items
			if (string.IsNullOrEmpty(input))
			{
				TileView.DataSource.AddRange(DataSource);
			}
			else
			{
				foreach (var item in DataSource)
				{
					// add item if match input
					if (IsMatch(item, input))
					{
						TileView.DataSource.Add(item);
					}
				}
			}

			TileView.DataSource.EndUpdate();
		}
	}
}