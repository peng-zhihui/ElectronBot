namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// DirectoryTreeView
	/// </summary>
	public class DirectoryTreeView : TreeViewCustom<DirectoryTreeViewComponent, FileSystemEntry>
	{
		/// <summary>
		/// Path separators.
		/// </summary>
		protected static char[] PathSeparators = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

		/// <summary>
		/// Root directory, if not specified drives will be used as root.
		/// </summary>
		[SerializeField]
		protected string rootDirectory = string.Empty;

		/// <summary>
		/// Root directory, if not specified drives will be used as root.
		/// </summary>
		public string RootDirectory
		{
			get
			{
				return rootDirectory;
			}

			set
			{
				SetRootDirectory(value);
			}
		}

		/// <summary>
		/// Display IO errors.
		/// </summary>
		[SerializeField]
		public IOExceptionsView ExceptionsView;

		bool isInited;

		/// <inheritdoc/>
		public override void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			base.Init();

			SetRootDirectory(rootDirectory);
		}

		/// <inheritdoc/>
		protected override void NodesChanged()
		{
			if (Nodes != null)
			{
				Nodes.OnChange -= NodesChanged;

				foreach (var node in Nodes)
				{
					EnsureSubNodesLoaded(node);
				}

				Nodes.OnChange += NodesChanged;
			}

			base.NodesChanged();
		}

		/// <summary>
		/// Ensure that sub-nodes data loaded for expanded nodes.
		/// </summary>
		/// <param name="node">Node.</param>
		protected virtual void EnsureSubNodesLoaded(TreeNode<FileSystemEntry> node)
		{
			if (!node.IsExpanded)
			{
				return;
			}

			var nodes = node.Nodes;
			nodes.BeginUpdate();

			// force update because node observation was disabled and if nothing found update will not be called
			nodes.CollectionChanged();

			LoadNodes(nodes);

			foreach (var n in nodes)
			{
				EnsureSubNodesLoaded(n);
			}

			nodes.EndUpdate();
		}

		/// <summary>
		/// Normalize path.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <returns>Normalized path.</returns>
		public static string NormalizePath(string path)
		{
			return Path.GetFullPath(new Uri(path).LocalPath).TrimEnd(PathSeparators);
		}

		/// <summary>
		/// Expand nodes to the specified path.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="scrollToNode">Scroll to nearest founded node.</param>
		/// <returns>The nearest founded node.</returns>
		public virtual TreeNode<FileSystemEntry> ExpandPath(string path, bool scrollToNode = true)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			path = NormalizePath(path);
			var nearest = NormalizedPath2NearestNode(path);

			if (scrollToNode && (nearest != null))
			{
				ScrollTo(nearest);
			}

			return nearest;
		}

		/// <summary>
		/// Get node with specified path.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <returns>Node.</returns>
		public virtual TreeNode<FileSystemEntry> Path2Node(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			path = NormalizePath(path);
			var nearest = NormalizedPath2NearestNode(path);
			if ((nearest != null) && (nearest.Item.FullName == path))
			{
				return nearest;
			}

			return null;
		}

		/// <summary>
		/// Get nearest node to the specified path.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <returns>Node.</returns>
		public virtual TreeNode<FileSystemEntry> Path2NearestNode(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			path = NormalizePath(path);
			return Path2NearestNode(path);
		}

		/// <summary>
		/// Get nearest node to the specified normalized path.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <returns>Node.</returns>
		protected virtual TreeNode<FileSystemEntry> NormalizedPath2NearestNode(string path)
		{
			var start_index = 1;
			var nodes = Nodes;

			TreeNode<FileSystemEntry> result = null;

			var start = string.Empty;

			while (start.Length < path.Length)
			{
				start = StartOfPath(path, start_index);
				start_index = start.Length + 1;

				var node = FindNodeByFullName(nodes, start);
				if (node != null)
				{
					result = node;
					node.IsExpanded = node.Item.FullName != path;
					nodes = node.Nodes;
				}
			}

			return result;
		}

		TreeNode<FileSystemEntry> FindNodeByFullName(ObservableList<TreeNode<FileSystemEntry>> nodes, string name)
		{
			foreach (var node in nodes)
			{
				if (node.Item.FullName == name)
				{
					return node;
				}
			}

			return null;
		}

		/// <summary>
		/// Get start part of the path after specified index.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="start">Start index.</param>
		/// <returns>Path of the path.</returns>
		protected string StartOfPath(string path, int start)
		{
			var pos = path.IndexOf(Path.DirectorySeparatorChar, start);
			if (pos == -1)
			{
				pos = path.Length;
			}

			var alt_pos = path.IndexOf(Path.AltDirectorySeparatorChar, start);
			if (alt_pos == -1)
			{
				alt_pos = path.Length;
			}

			pos = Mathf.Min(pos, alt_pos);

			// because *nix paths starts with /
			if (pos == 0)
			{
				return path.Substring(0, 1);
			}

			if (path[pos - 1] == ':')
			{
				pos += 1;
			}

			if (pos >= path.Length)
			{
				return path;
			}

			return path.Substring(0, pos);
		}

		/// <summary>
		/// Set root directory.
		/// </summary>
		/// <param name="root">New root.</param>
		protected virtual void SetRootDirectory(string root)
		{
			rootDirectory = root;

			if (string.IsNullOrEmpty(root))
			{
				var drives = GetDrives();
				LoadNodes(drives);

				Nodes = drives;
			}
			else
			{
				var nodes = GetDirectoriesNodes(root);
				LoadNodes(nodes);

				Nodes = nodes;
			}
		}

		/// <summary>
		/// Get directories list from current to root.
		/// </summary>
		/// <param name="directory">Directory.</param>
		/// <returns>Directories list from current to root.</returns>
		protected static List<string> GetPaths(string directory)
		{
			var paths = new List<string>();
			var temp = directory;

			do
			{
				paths.Add(temp);
				temp = Path.GetDirectoryName(temp);
			}
			while (!string.IsNullOrEmpty(temp));

			return paths;
		}

		/// <summary>
		/// Get node for specified directory.
		/// </summary>
		/// <param name="directory">Directory.</param>
		/// <returns>Node if directory found; otherwise, null.</returns>
		public virtual TreeNode<FileSystemEntry> GetNodeByPath(string directory)
		{
			if (!Directory.Exists(directory))
			{
				return null;
			}

			var paths = GetPaths(directory);
			var nodes = Nodes;
			TreeNode<FileSystemEntry> node;

			do
			{
				node = FindNode(nodes, paths);
				if (node == null)
				{
					return null;
				}

				paths.Remove(node.Item.FullName);

				if (node.Nodes == null)
				{
					node.Nodes = GetDirectoriesNodes(node.Item.FullName);
				}

				nodes = node.Nodes;
			}
			while (directory != node.Item.FullName);

			return node;
		}

		/// <summary>
		/// Find node with one of the specified paths.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		/// <param name="paths">Paths.</param>
		/// <returns>Node.</returns>
		protected virtual TreeNode<FileSystemEntry> FindNode(ObservableList<TreeNode<FileSystemEntry>> nodes, List<string> paths)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				if (paths.Contains(nodes[i].Item.FullName))
				{
					return nodes[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Select node with specified directory.
		/// </summary>
		/// <param name="directory">Directory.</param>
		/// <returns>true if directory found; otherwise, false.</returns>
		public virtual bool SelectDirectory(string directory)
		{
			return SelectDirectory(directory, true);
		}

		/// <summary>
		/// Select node with specified directory.
		/// </summary>
		/// <param name="directory">Directory.</param>
		/// <param name="scrollToDirectory">Scroll to directory.</param>
		/// <returns>true if directory found; otherwise, false.</returns>
		public virtual bool SelectDirectory(string directory, bool scrollToDirectory)
		{
			var node = GetNodeByPath(directory);
			if (node == null)
			{
				return false;
			}

			var parent = node.Parent;
			while (parent != null)
			{
				parent.IsExpanded = true;
				LoadNodes(parent.Nodes);
				parent = parent.Parent;
			}

			Select(node);

			if (scrollToDirectory)
			{
				ScrollTo(node);
			}

			return true;
		}

		/// <summary>
		/// Ger drives.
		/// </summary>
		/// <returns>Drives list.</returns>
		public virtual ObservableList<TreeNode<FileSystemEntry>> GetDrives()
		{
			return ExceptionsView.Execute<ObservableList<TreeNode<FileSystemEntry>>>(FillDrivesList);
		}

		/// <summary>
		/// Fill the drives list.
		/// </summary>
		/// <param name="list">list.</param>
		protected virtual void FillDrivesList(ObservableList<TreeNode<FileSystemEntry>> list)
		{
#if !NETFX_CORE
			var drives = Directory.GetLogicalDrives();
			for (int i = 0; i < drives.Length; i++)
			{
				var item = new FileSystemEntry(drives[i], drives[i], false);
				list.Add(new TreeNode<FileSystemEntry>(item, null));
			}
#endif
		}

		/// <summary>
		/// Load sub-nodes data for specified nodes.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		public virtual void LoadNodes(ObservableList<TreeNode<FileSystemEntry>> nodes)
		{
			nodes.BeginUpdate();

			try
			{
				foreach (var node in nodes)
				{
					LoadNode(node);
				}
			}
			finally
			{
				nodes.EndUpdate();
			}
		}

		/// <summary>
		/// Get sub-nodes for specified directory.
		/// </summary>
		/// <param name="path">Directory.</param>
		/// <returns>Sub-nodes for specified directory.</returns>
		public virtual ObservableList<TreeNode<FileSystemEntry>> GetDirectoriesNodes(string path)
		{
			var nodes = ExceptionsView.Execute<ObservableList<TreeNode<FileSystemEntry>>, string>(FillDirectoriesList, path);
			ExceptionsView.CurrentError = null;

			return nodes;
		}

		/// <summary>
		/// Fill the directories list.
		/// </summary>
		/// <param name="list">List.</param>
		/// <param name="path">Path.</param>
		protected virtual void FillDirectoriesList(ObservableList<TreeNode<FileSystemEntry>> list, string path)
		{
			var directories = Directory.GetDirectories(path);
			for (int i = 0; i < directories.Length; i++)
			{
				var item = new FileSystemEntry(directories[i], Path.GetFileName(directories[i]), false);
				list.Add(new TreeNode<FileSystemEntry>(item, null));
			}
		}

		/// <summary>
		/// Load sub-nodes data for specified node.
		/// </summary>
		/// <param name="node">Node.</param>
		public virtual void LoadNode(TreeNode<FileSystemEntry> node)
		{
			if (node.Nodes != null)
			{
				return;
			}

			node.Nodes = GetDirectoriesNodes(node.Item.FullName);
		}

		#region RefreshDirectories

		/// <summary>
		/// Previously selected directories.
		/// </summary>
		protected List<string> PreviouslySelectedDirectories = new List<string>();

		/// <summary>
		/// Previously expanded directories.
		/// </summary>
		protected List<string> PreviouslyExpandedDirectories = new List<string>();

		/// <summary>
		/// Previous directory at the scroll position.
		/// </summary>
		protected string PreviousDirectoryAtScroll;

		/// <summary>
		/// Previous scroll position.
		/// </summary>
		protected float PreviousScrollPosition;

		/// <summary>
		/// Save expanded directories.
		/// </summary>
		protected void SaveExpandedDirectories()
		{
			PreviouslyExpandedDirectories.Clear();

			for (int i = 0; i < DataSource.Count; i++)
			{
				var node = DataSource[i].Node;
				if (node.IsExpanded)
				{
					PreviouslyExpandedDirectories.Add(node.Item.FullName);
				}
			}

			PreviouslyExpandedDirectories.Sort();
		}

		/// <summary>
		/// Save selected directories.
		/// </summary>
		protected void SaveSelectedDirectories()
		{
			PreviouslySelectedDirectories.Clear();

			foreach (var node in selectedNodes)
			{
				PreviouslySelectedDirectories.Add(node.Item.FullName);
			}

			PreviouslySelectedDirectories.Sort();
		}

		/// <summary>
		/// Save scroll position.
		/// </summary>
		protected void SaveScrollPosition()
		{
			PreviousDirectoryAtScroll = (DisplayedIndices.Count > 1)
				? DataSource[DisplayedIndices[0]].Node.Item.FullName
				: null;

			PreviousScrollPosition = GetScrollPosition();
		}

		/// <summary>
		/// Restore expanded directories.
		/// </summary>
		protected void RestoreExpandedDirectories()
		{
			foreach (var path in PreviouslyExpandedDirectories)
			{
				var node = ExpandPath(path, false);
				if (node != null)
				{
					node.IsExpanded = true;
				}
			}

			PreviouslyExpandedDirectories.Clear();
		}

		/// <summary>
		/// Restore selected directories.
		/// </summary>
		protected void RestoreSelectedDirectories()
		{
			foreach (var path in PreviouslySelectedDirectories)
			{
				var node = ExpandPath(path, false);
				if (node != null)
				{
					Select(node);
				}
			}

			PreviouslySelectedDirectories.Clear();
		}

		/// <summary>
		/// Restore scroll position.
		/// </summary>
		protected void RestoreScrollPosition()
		{
			var node = ExpandPath(PreviousDirectoryAtScroll, false);
			var index = (node != null) ? Node2Index(node) : -1;

			if (IsValid(index))
			{
				var pos = GetItemPosition(index);
				ScrollToPosition(pos);
			}
			else
			{
				ScrollToPosition(PreviousScrollPosition);
			}

			PreviousDirectoryAtScroll = null;
		}

		/// <summary>
		/// Refresh displayed directories according to current state of the file system.
		/// </summary>
		public void RefreshDirectories()
		{
			SaveExpandedDirectories();
			SaveSelectedDirectories();
			SaveScrollPosition();

			SetRootDirectory(RootDirectory);

			RestoreExpandedDirectories();
			RestoreSelectedDirectories();
			RestoreScrollPosition();
		}
		#endregion

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			if (ExceptionsView != null)
			{
				ExceptionsView.SetStyle(style);
			}

			return base.SetStyle(style);
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			if (ExceptionsView != null)
			{
				ExceptionsView.GetStyle(style);
			}

			return base.GetStyle(style);
		}
		#endregion
	}
}