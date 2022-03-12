namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// TreeViewSample data.
	/// </summary>
	public class TreeViewSampleData : MonoBehaviour
	{
		/// <summary>
		/// TreeViewSample.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("tree")]
		protected TreeViewSample TreeViewSample;

		/// <summary>
		/// Europe data.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("dataEurope")]
		protected List<TreeViewSampleDataCountry> DataEurope = new List<TreeViewSampleDataCountry>();

		/// <summary>
		/// Asia data.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("dataAsia")]
		protected List<TreeViewSampleDataCountry> DataAsia = new List<TreeViewSampleDataCountry>();

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			TreeViewSample.Init();

			TreeViewSample.Nodes = GetData();
		}

		/// <summary>
		/// Get data.
		/// </summary>
		/// <returns>Nodes.</returns>
		protected virtual ObservableList<TreeNode<ITreeViewSampleItem>> GetData()
		{
			var countries = new ObservableList<TreeNode<ITreeViewSampleItem>>
			{
				Node(new TreeViewSampleItemContinent("Africa", 54)),
				Node(new TreeViewSampleItemContinent("Antarctica", 12)),
				Node(new TreeViewSampleItemContinent("Asia", 48), Data2Country(DataAsia)),
				Node(new TreeViewSampleItemContinent("Australia", 4)),
				Node(new TreeViewSampleItemContinent("Europe", 50), Data2Country(DataEurope)),
				Node(new TreeViewSampleItemContinent("North America", 23)),
				Node(new TreeViewSampleItemContinent("South America", 12)),
			};

			return countries;
		}

		/// <summary>
		/// Convert list to nodes.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <returns>Nodes.</returns>
		protected static ObservableList<TreeNode<ITreeViewSampleItem>> Data2Country(List<TreeViewSampleDataCountry> data)
		{
			var countries = new ObservableList<TreeNode<ITreeViewSampleItem>>();
			foreach (var item in data)
			{
				countries.Add(Node(new TreeViewSampleItemCountry(item.Name, item.Flag)));
			}

			return countries;
		}

		/// <summary>
		/// Create node with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="nodes">Subnodes.</param>
		/// <returns>Node.</returns>
		protected static TreeNode<ITreeViewSampleItem> Node(ITreeViewSampleItem item, ObservableList<TreeNode<ITreeViewSampleItem>> nodes = null)
		{
			return new TreeNode<ITreeViewSampleItem>(item, nodes, false, true);
		}
	}
}