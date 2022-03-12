namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// File with data for ListViewString.
	/// </summary>
	[RequireComponent(typeof(ListViewString))]
	public class ListViewStringDataFile : MonoBehaviour
	{
		[NonSerialized]
		ListViewString listView;

		/// <summary>
		/// ListView.
		/// </summary>
		public ListViewString ListView
		{
			get
			{
				if (listView == null)
				{
					listView = GetComponent<ListViewString>();
				}

				return listView;
			}
		}

		[SerializeField]
		TextAsset file;

		/// <summary>
		/// Gets or sets the file with strings for ListView. One string per line.
		/// </summary>
		/// <value>The file.</value>
		public TextAsset File
		{
			get
			{
				return file;
			}

			set
			{
				file = value;
				if (file != null)
				{
					ListView.DataSource = GetItemsFromFile(file);
					ListView.ScrollToPosition(0);
				}
			}
		}

		/// <summary>
		/// The comments in file start with specified strings.
		/// </summary>
		[SerializeField]
		public List<string> CommentsStartWith = new List<string>() { "#", "//" };

		/// <summary>
		/// Allow only unique strings.
		/// </summary>
		[SerializeField]
		public bool Unique = true;

		/// <summary>
		/// Allow empty strings.
		/// </summary>
		[SerializeField]
		public bool AllowEmptyItems;

		/// <summary>
		/// Create a new list.
		/// </summary>
		[SerializeField]
		public bool CreateNewList;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			if (ListView == null)
			{
				return;
			}

			if (File != null)
			{
				var items = GetItemsFromFile(File);
				items.Comparison = ListView.DataSource.Comparison;

				if (CreateNewList)
				{
					ListView.DataSource = items;
				}
				else
				{
					ListView.DataSource.Clear();
					ListView.DataSource.AddRange(items);
				}

				ListView.ScrollToPosition(0);
			}
		}

		/// <summary>
		/// Gets the items from file.
		/// </summary>
		/// <param name="sourceFile">Source file.</param>
		/// <returns>Items list.</returns>
		public ObservableList<string> GetItemsFromFile(TextAsset sourceFile)
		{
			var result = new ObservableList<string>();

			if (file == null)
			{
				result.Comparison = ListView.ItemsComparison;
				return result;
			}

			foreach (var str in sourceFile.text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
			{
				var item = str.TrimEnd();

				if (Unique && result.Contains(item))
				{
					continue;
				}

				if (!AllowEmptyItems && string.IsNullOrEmpty(item))
				{
					continue;
				}

				if (IsComment(item))
				{
					continue;
				}

				result.Add(item);
			}

			return result;
		}

		/// <summary>
		/// Is comment?
		/// </summary>
		/// <param name="item">Item to check.</param>
		/// <returns>true if item is comment; otherwise false.</returns>
		protected bool IsComment(string item)
		{
			foreach (var comment in CommentsStartWith)
			{
				if (UtilitiesCompare.StartsWith(item, comment))
				{
					return true;
				}
			}

			return false;
		}
	}
}