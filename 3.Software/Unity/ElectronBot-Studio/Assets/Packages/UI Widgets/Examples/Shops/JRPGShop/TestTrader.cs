namespace UIWidgets.Examples.Shops
{
	using System.Collections.Generic;
	using System.IO;
#if !NETFX_CORE
	using System.Runtime.Serialization.Formatters.Binary;
#endif
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Test trader.
	/// </summary>
	public class TestTrader : MonoBehaviour
	{
		/// <summary>
		/// TraderListView.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("List")]
		protected TraderListView TraderListView;

		/// <summary>
		/// TraderTreeView.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("Tree")]
		protected TraderTreeView TraderTreeView;

		/// <summary>
		/// Create OrderLines.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <returns>OrderLines.</returns>
		protected static ObservableList<JRPGOrderLine> CreateOrderLines(List<Item> items)
		{
			var result = new ObservableList<JRPGOrderLine>();

			foreach (var item in items)
			{
				result.Add(new JRPGOrderLine(item, Prices.GetPrice(item, 1f)));
			}

			return result;
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected void Start()
		{
			Init();
		}

		/// <summary>
		/// Test scroll to and select last node.
		/// </summary>
		public void TestScrollAndSelectNode()
		{
			TestScrollAndSelectNode(TraderTreeView.DataSource.Count - 1);
		}

		/// <summary>
		/// Test scroll to and select node by specified index.
		/// </summary>
		/// <param name="index">Node index.</param>
		public void TestScrollAndSelectNode(int index)
		{
			var node = TraderTreeView.DataSource[index].Node;

			// expand parent nodes
			TraderTreeView.ExpandParentNodes(node);

			// select node
			TraderTreeView.Select(node);

			// check if visible
			if (!TraderTreeView.IsVisible(TraderTreeView.SelectedIndex))
			{
				// scroll to it
				TraderTreeView.ScrollToAnimated(TraderTreeView.SelectedIndex);
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected void Init()
		{
			var list_items = new List<Item>()
			{
				new Item("Sword", 10),
				new Item("Short Sword", 5),
				new Item("Long Sword", 5),
				new Item("Knife", -1),
				new Item("Dagger", -1),
				new Item("Hammer", -1),
				new Item("Shield", -1),
				new Item("Leather Armor", 3),
				new Item("Ring", 2),
				new Item("Bow", -1),
				new Item("Crossbow", -1),

				new Item("HP Potion", -1),
				new Item("Mana Potion", -1),
				new Item("HP UP", 10),
				new Item("Mana UP", 10),
			};
			TraderListView.DataSource.AddRange(CreateOrderLines(list_items));

			var nodes = new ObservableList<TreeNode<JRPGOrderLine>>();

			var playlist1 = new JRPGOrderLine(new Item("Playlist 1", 0), 0) { IsPlaylist = true };
			var node1 = new TreeNode<JRPGOrderLine>(playlist1);
			nodes.Add(node1);

			var playlist2 = new JRPGOrderLine(new Item("Playlist 2", 0), 0) { IsPlaylist = true };
			var node2 = new TreeNode<JRPGOrderLine>(playlist2);
			nodes.Add(node2);

			TraderTreeView.Nodes = nodes;
		}

		/// <summary>
		/// AddPlaylistDialog template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("addPlaylistDialog")]
		protected Dialog AddPlaylistDialogTemplate;

		/// <summary>
		/// Add playlist by name.
		/// </summary>
		/// <param name="name">Dialog name.</param>
		public void AddPlaylist(string name)
		{
			var playlist = new JRPGOrderLine(new Item(name, 0), 0) { IsPlaylist = true };
			TraderTreeView.Nodes.Add(new TreeNode<JRPGOrderLine>(playlist));
		}

		/// <summary>
		/// Open AddPlaylistDialog.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0302:Display class allocation to capture closure", Justification = "Required.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0301:Closure Allocation Source", Justification = "Required.")]
		public void AddPlaylistDialog()
		{
			// create dialog from template
			var dialog = AddPlaylistDialogTemplate.Clone();

			// helper component with references to input fields
			var helper = dialog.GetComponent<PlaylistDialogHelper>();

			// reset input fields to default
			helper.Refresh();

			var actions = new DialogButton[]
			{
				new DialogButton("Add", CheckPlaylistDialog),

				// on click close dialog
				new DialogButton("Cancel", DialogBase.DefaultClose),
			};

			// open dialog
			dialog.Show(
				title: "Add playlist",
				buttons: actions,
				focusButton: "Sign in",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f));
		}

		/// <summary>
		/// Check AddPlaylistDialog.
		/// </summary>
		/// <param name="dialog">Dialog.</param>
		/// <param name="index">Button index.</param>
		/// <returns>true if values valid; otherwise false.</returns>
		protected bool CheckPlaylistDialog(DialogBase dialog, int index)
		{
			var helper = dialog.GetComponent<PlaylistDialogHelper>();
			if (!helper.Validate())
			{
				// return false to keep dialog open
				return false;
			}

			AddPlaylist(helper.NameAdapter.text);

			// return true to close dialog
			return true;
		}

		/// <summary>
		/// The filename.
		/// </summary>
		[SerializeField]
		protected string Filename = "TestTrader.binary";

		/// <summary>
		/// Test saving with binary serialization.
		/// </summary>
		public void TestSave()
		{
			#if NETFX_CORE
			Debug.Log("UWP does not support binary formatters. Saving test not available.", this);
			#else
			var raw_data = TreeNode<JRPGOrderLine>.Serialize(TraderTreeView.Nodes);

			var stream = File.Create(Filename);
			var serializer = new BinaryFormatter();
			serializer.Serialize(stream, raw_data);
			stream.Close();
			#endif
		}

		/// <summary>
		/// Test loading with binary serialization.
		/// </summary>
		public void TestLoad()
		{
			#if NETFX_CORE
			Debug.Log("UWP does not support binary formatters. Loading test not available.", this);
			#else
			if (File.Exists(Filename))
			{
				// binary save
				var stream = File.OpenRead(Filename);
				var deserializer = new BinaryFormatter();
				var raw_data = (List<TreeNodeSerialized<JRPGOrderLine>>)deserializer.Deserialize(stream);
				stream.Close();

				TraderTreeView.Nodes = TreeNode<JRPGOrderLine>.Deserialize(raw_data);
			}
			#endif
		}

		/// <summary>
		/// Test Easy Save 3 saving.
		/// </summary>
		/// <example>
		/// var raw_data = TreeNode{JRPGOrderLine}.Serialize(Tree.Nodes);
		/// ES3.Save{List{TreeNodeSerialized{JRPGOrderLine}}}("playlists", raw_data, "saved.txt");
		/// </example>
		public void TestES3Save()
		{
			Debug.Log("Easy Save 3 required", this);
		}

		/// <summary>
		/// Test Easy Save 3 loading.
		/// </summary>
		/// <example>
		/// var raw_data = ES3.Load{List{TreeNodeSerialized{JRPGOrderLine}}}("saved.txt?tag=playlists");
		/// Tree.Nodes = TreeNode{JRPGOrderLine}.Deserialize(raw_data);
		/// </example>
		public void TestES3Load()
		{
			Debug.Log("Easy Save 3 required", this);
		}
	}
}